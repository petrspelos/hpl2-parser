using System;
using SampleLexer.Core;
using SampleLexer.Tests.Mocks;
using Xunit;

namespace SampleLexer.Tests
{
    public class HplParserTests
    {
        private readonly HplTokenizerMock _tokensMock;
        private readonly HplParser _parser;

        public HplParserTests()
        {
            _tokensMock = new();
            _parser = new(_tokensMock);
        }
        
        [Fact]
        public void Test1()
        {

        }
    }
}
