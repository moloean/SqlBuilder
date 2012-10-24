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

        [Test]
        public void ToString_WhenWhereIsCalledWithArgument_returnTheSqlExpesonWithArgumetn()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();
            sqlBuilder.Select("*")
                .From("Person").Where("Name = 'Morten'");

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.EqualTo("SELECT * FROM Person WHERE Name = 'Morten'"));
        }

    }



    public class SqlStatement
    {
        private readonly StringBuilder _statement = new StringBuilder();
        public void AddToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;

            if (_statement.Length > 0)
                _statement.Append(" ");

            _statement.Append(token.Trim());
        }

        public override string ToString()
        {
            return _statement.ToString();
        }
    }

    public abstract class SqlKeyword
    {
        protected SqlKeyword() {}

        protected SqlKeyword(SqlStatement sqlStatement)
        {
            SqlStatement = sqlStatement;
        }

        private SqlStatement SqlStatement { get; set; }
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

        private void Init(SqlStatement sqlStatement, string arg)
        {
            SqlStatement = sqlStatement;
            
            if(!string.IsNullOrWhiteSpace(Name))
                SqlStatement.AddToken(Name.ToUpper());

            SqlStatement.AddToken(arg);
        }
    }

    public class SqlBuilder : SqlKeyword
    {
        protected override string Name{get { return string.Empty; }}

        public SqlBuilder():base(new SqlStatement()){}

        public SqlSelect Select(string arg)
        {
            return AddKeyword<SqlSelect>(arg);
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

        public SqlWhere Where(string arg)
        {
           return AddKeyword<SqlWhere>(arg);
        }
    }

    public class SqlWhere : SqlKeyword
    {
        protected override string Name
        {
            get { return "WHERE"; }
        }
    }
}