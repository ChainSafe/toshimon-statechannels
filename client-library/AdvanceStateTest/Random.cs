namespace test;

public class RandomTests
{
    [Theory]
    [InlineData(1, 2, 0)]
    [InlineData(1, 2, 1)]
    [InlineData(1, 2, 2)]
    [InlineData(1, 2, 3)]
    [InlineData(1, 2, 4)]
    [InlineData(1, 2, 5)]
    public void UniformBetweenMinAndMax(uint min, uint max, uint randomSeed) {
        uint rval = DetRandom.uniform(min, max, randomSeed, 0);
        Assert.True(rval >= min && rval <= max);   
    }
}
