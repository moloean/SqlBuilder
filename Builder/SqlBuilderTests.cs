using System.Text;
using NUnit.Framework;

namespace Builder
{
    [TestFixture]
    public class SqlBuilderTests
    {
        [Test]
        public void ToString_Init_ReturnStringEmpty()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.Empty.And.Not.Null);
        }

        [Test]
        public void ToString_WhenSelectIsCallWithArgument_ReturnSelectWithArgument()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();
            sqlBuilder.Select("*");

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.EqualTo("SELECT *"));
        }
    }

    public class SqlBuilder
    {
        private string _sqlStatement = string.Empty;

        public override string ToString()
        {
            return _sqlStatement;
        }

        public void Select(string arg)
        {
            _sqlStatement = string.Format("{0} {1}", "SELECT", arg);
        }
    }
}