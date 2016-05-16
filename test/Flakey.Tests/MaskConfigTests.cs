using System;
using Xunit;

namespace Flakey
{
    public class MaskConfigTests
    {
        [Fact]
        public void DefaultMaskConfig_Matches_Expectations()
        {
            var m = MaskConfig.Default;

            Assert.Equal(41, m.TimestampBits);
            Assert.Equal(10, m.GeneratorIdBits);
            Assert.Equal(12, m.SequenceBits);
            Assert.Equal(63, m.TotalBits);

            // We should be able to generate a total of 63 bits worth of Id's
            Assert.Equal(long.MaxValue, (m.MaxGenerators * m.MaxIntervals * m.MaxSequenceIds) - 1);
        }

        [Fact]
        public void MaskConfig_CalculatesWraparoundInterval_Correctly()
        {
            // 40 bits of Timestamp should give us about 34 years worth of Id's
            Assert.Equal(34, (int)(new MaskConfig(40, 11, 12).WraparoundInterval().TotalDays / 365.25));
            // 41 bits of Timestamp should give us about 69 years worth of Id's
            Assert.Equal(69, (int)(new MaskConfig(41, 11, 11).WraparoundInterval().TotalDays / 365.25));
            // 42 bits of Timestamp should give us about 139 years worth of Id's
            Assert.Equal(139, (int)(new MaskConfig(42, 11, 10).WraparoundInterval().TotalDays / 365.25));
        }

        [Fact]
        public void MaskConfig_CalculatesWraparoundDate_Correctly()
        {
            var m = MaskConfig.Default;
            var d = m.WraparoundDate(new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc));
            Assert.Equal(new DateTime(643346200555520000, DateTimeKind.Utc), d);
        }
    }
}
