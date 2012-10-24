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

        protected T AddKeyword<T>(string arg) 
            where T : SqlKeyword, new()
        {
            var sqlKeyword = new T();
            sqlKeyword.Init(SqlStatement, arg);

            return sqlKeyword;
        }

        private void AddToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;

            if (SqlStatement.Length > 0)
                SqlStatement.Append(" ");

            SqlStatement.Append(token.Trim());

        }

        private void Init(StringBuilder sqlStatement, string arg)
        {
            SqlStatement = sqlStatement;
            
            if(!string.IsNullOrWhiteSpace(Name))
                AddToken(Name.ToUpper());
            
            AddToken(arg);
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
            return AddKeyword<SqlSelect>(arg);
        }

        protected override string Name
        {
            get { return string.Empty; }
        }
    }

    public class SqlSelect : SqlKeyword
    {
        protected override string Name {get { return "SELECT"; }}
        
        public SqlFrom From(string arg)
        {
            return AddKeyword<SqlFrom>(arg);
        }
        
    }

    public class SqlFrom : SqlKeyword
    {
       protected override string Name
        {
            get { return "FROM"; }
        }
    }
}