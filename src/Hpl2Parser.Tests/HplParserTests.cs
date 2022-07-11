using System;
using Hpl2Parser.Core;
using Hpl2Parser.Core.Parsing;
using Hpl2Parser.Tests.Mocks;
using Xunit;

namespace Hpl2Parser.Tests
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
