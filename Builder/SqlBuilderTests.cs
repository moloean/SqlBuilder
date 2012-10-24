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
        public void ToString_WhenWhereIsCalledWithArgument_returnTheSqlExpesonAndAppendWithAndArgumetn()
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

        [Test]
        public void ToString_WhenCalledInner_AddInnerToTheSqlExpreson()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();
            var sqlInner = sqlBuilder.Select("*").From("Person").Inner;

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.EqualTo("SELECT * FROM Person INNER"));
        }


        [Test]
        public void ToString_WhenCalledJoin_AddJoinToTheSqlExpreson()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();
            var sqlInner = sqlBuilder.Select("*").From("Person")
                .Inner.Join("Address");

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.EqualTo("SELECT * FROM Person INNER JOIN Address"));
        }

        [Test]
        public void ToString_WhenCalledOn_AddOnToTheSqlExpreson()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();
            sqlBuilder.Select("*").From("Person")
                .Inner.Join("Address").On("Address.PostCode = Person.PostCode");

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.EqualTo("SELECT * FROM Person INNER JOIN Address ON Address.PostCode = Person.PostCode"));
        }

        [Test]
        public void ToString_WhenCalledWhereAfterOn_AddWherToTheSqlExpreson()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();
            sqlBuilder.Select("*").From("Person")
                .Inner.Join("Address").On("Address.PostCode = Person.PostCode")
                .Where("Name = 'Morten'");

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.EqualTo("SELECT * FROM Person INNER JOIN Address ON Address.PostCode = Person.PostCode WHERE Name = 'Morten'"));
        }


        [Test]
        public void ToString_WhenCalledInnerAfterOn_AddJoinToTheSqlExpreson()
        {
            // Arrange
            var sqlBuilder = new SqlBuilder();
            sqlBuilder.Select("*").From("Person")
                .Inner.Join("Address").On("Address.PostCode = Person.PostCode")
                .Inner.Join("Table2").On("Table2.PersonId = Person.Id");
                

            // Act
            var sqlExpresion = sqlBuilder.ToString();

            // Assert
            Assert.That(sqlExpresion, Is.EqualTo("SELECT * FROM Person INNER JOIN Address ON Address.PostCode = Person.PostCode INNER JOIN Table2 ON Table2.PersonId = Person.Id"));
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

        protected T AddKeyword<T>()
            where T : SqlKeyword, new()
        {
            return AddKeyword<T>(string.Empty);
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

        public SqlInner Inner
        {
            get { return AddKeyword<SqlInner>(); }
            
        }

        public SqlWhere Where(string arg)
        {
           return AddKeyword<SqlWhere>(arg);
        }
    }

    public class SqlInner : SqlKeyword
    {
        protected override string Name
        {
            get { return "INNER"; }
        }

        public SqlJoin Join(string tableName)
        {
            return AddKeyword<SqlJoin>(tableName);
        }
    }

    public class SqlJoin : SqlKeyword
    {
        protected override string Name
        {
            get { return "JOIN"; }
        }

        public SqlOn On(string joinPredicate)
        {
            return AddKeyword<SqlOn>(joinPredicate);
        }
    }

    public class SqlOn : SqlKeyword
    {
        protected override string Name
        {
            get { return "ON"; }
        }

        public SqlInner Inner {get { return AddKeyword<SqlInner>(); }}
        public SqlWhere Where(string arg) {return AddKeyword<SqlWhere>(arg);}
    }

    public class SqlWhere : SqlKeyword
    {
        protected override string Name
        {
            get { return "WHERE"; }
        }
    }
}