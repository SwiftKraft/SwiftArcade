namespace SwiftArcadeMode.Utils.Voice
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using VoiceChat.Codec;
    using VoiceChat.Networking;

    public static class VoiceDecoding
    {
        private static readonly Dictionary<ReferenceHub, OpusDecoder> Decoders = [];

        private static readonly float[] ReadBuffer = new float[24000];

        public static ArraySegment<float> ToPcm(this VoiceMessage message)
        {
            if (!Decoders.TryGetValue(message.Speaker, out OpusDecoder decoder))
            {
                decoder = Decoders[message.Speaker] = new OpusDecoder();
            }

            int len = decoder.Decode(message.Data, message.DataLength, ReadBuffer);

            return ReadBuffer.Segment(0, len);
        }

        public static double CalculateRMS(ArraySegment<float> pcmData)
        {
            if (pcmData.Count == 0)
                return 0F;

            double sumOfSquares = pcmData.Aggregate(0D, (current, sample) => current + (sample * sample));

            double meanSquare = sumOfSquares / pcmData.Count;
            return Math.Sqrt(meanSquare);
        }

        public static double CalculateLoudnessDB(ArraySegment<float> pcmData)
        {
            if (pcmData.Count == 0)
                return -96f; // Silence in dB

            double rms = CalculateRMS(pcmData);

            // Convert to dBFS (decibels relative to full scale)
            // Avoid log(0) by using a very small value
            if (rms < 1e-6f)
                return -96f;

            return 20f * Math.Log10(rms);
        }
    }
}
