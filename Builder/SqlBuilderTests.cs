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

        [Test]
        public void ToString_WhenFromIsCalledWithArgument_returnTheSqlExpresonWithFromAndArgument()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();
            sqlBuilder.Select("*")
                .From("Person");

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.EqualTo("SELECT * FROM Person"));
        }

    }


    public abstract class SqlKeyword
    {
        protected StringBuilder SqlStatement { get; set; }

        protected abstract string Name { get; }

        public override string ToString()
        {
            return SqlStatement.ToString();
        }
    }

    public class SqlBuilder : SqlKeyword
    {
        public SqlBuilder()
        {
            SqlStatement = new StringBuilder();
        }

        public SqlSelect Select(string arg)
        {
            return new SqlSelect(SqlStatement, arg);
        }

        protected override string Name
        {
            get { return string.Empty; }
        }
    }

    public class SqlSelect : SqlKeyword
    {
        protected override string Name {get { return "SELECT"; }}

        public SqlSelect(StringBuilder sqlStatement, string arg)
        {
            SqlStatement = sqlStatement;
            SqlStatement.AppendFormat("{0} {1}", Name, arg);
        }

        public void From(string arg)
        {
            SqlStatement.AppendFormat(" {0} {1}", "FROM", arg);
        }

        
    }
}