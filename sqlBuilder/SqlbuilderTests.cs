using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using NUnit.Framework;

namespace LIMSoft
{
    [TestFixture]
    public class SqlbuilderTests
    {
        [Test]
        public void GivenANewSqlBuilder_ToStringReturnStringEmpty()
        {
            var sqlBuilder = new SqlBuilder();
            Assert.That(sqlBuilder.ToString(), Is.Empty);
        }

        [Test]
        public void GivenA_Select_ToStringReturnsSelect()
        {
            var sqlBuilder = new SqlBuilder();
            sqlBuilder
                .Select("*");

            Assert.That(sqlBuilder.ToString(), Is.EqualTo("SELECT *"));

        }

        [Test]
        public void FromTest()
        {
            var sqlBuilder = new SqlBuilder();
            sqlBuilder
                .Select("*")
                .From("TestTable");

            Assert.That(sqlBuilder.ToString(), Is.EqualTo("SELECT * FROM @TestTable"));
     
        }


        [Test]
        public void WhereTest()
        {
            var sqlBuilder = new SqlBuilder();
            sqlBuilder
                .Select("*")
                .From("TestTable")
                .Where("Name");

            Assert.That(sqlBuilder.ToString(), Is.EqualTo("SELECT * FROM @TestTable WHERE Name"));

        }

        [Test]
        public void EqTest()
        {
            var sqlBuilder = new SqlBuilder();
            sqlBuilder
                .Select("*")
                .From("TestTable")
                .Where("Name")
                .Eq(1);

            Assert.That(sqlBuilder.ToString(), Is.EqualTo("SELECT * FROM @TestTable WHERE Name = @p_0"));
        }

        [Test]
        public void AndTest()
        {
            var sqlBuilder = new SqlBuilder();
            sqlBuilder
                .Select("*")
                .From("TestTable")
                .Where("Name").Eq("Bouvet")
                .And("Age").Eq(10);

            Assert.That(sqlBuilder.ToString(), Is.EqualTo("SELECT * FROM @TestTable WHERE Name = @p_0 AND Age = @p_1"));
        }


        [Test]
        public void JonTest()
        {
            var sqlBuilder = new SqlBuilder();
            sqlBuilder
                .Select("*")
                .From("TestTable")
                .Outer.Join("blbal").On("safd").Eq(4)
                .Join("Table2").On("sdfdsf").Eq(2)
                    .And("afdasdf").Eq("teset")
                .Join("").On("").Eq(3)
                .Where("Name").Eq("Bouvet");
                

            Assert.That(sqlBuilder.ToString(), Is.EqualTo("SELECT * FROM @TestTable WHERE Name = @p_0 AND Age = @p_1"));
        }
    }

    public class SqlCommand
    {
        private int _numberOfUnameparemeters;
        private readonly StringBuilder _sqlStatement = new StringBuilder();
        IList<SqlParameter> _parameters = new List<SqlParameter>();
        public void AddToken(string token)
        {
            if (_sqlStatement.Length > 0)
                _sqlStatement.Append(" ");

            _sqlStatement.Append(token.Trim());
        }

        public void AddParameter<T>(string key, T value)
        {
            string parameterName = string.Format("@{0}", key.Trim());
            AddToken(parameterName);
            _parameters.Add(new SqlParameter(parameterName, value));
        }

        public override string ToString()
        {
            return _sqlStatement.ToString();
        }

       
        public void AddParameter<T>(T value)
        {
            string nextParemeterName = GetNextParemeterName();

            AddToken(nextParemeterName);
            _parameters.Add(new SqlParameter(nextParemeterName, value));
        }

        private string GetNextParemeterName()
        {
            return string.Format("@p_{0}", _numberOfUnameparemeters++);
        }
    }

    public class SqlBuilder
    {
        private SqlCommand _command = new SqlCommand();
    
        
        
        public override string ToString()
        {
            return _command.ToString();
        }

        public Select Select(string s)
        {
            return new Select(_command, s);
        }
    }

    public class Select
    {
        private readonly SqlCommand _command;

        public Select(SqlCommand command, string s)
        {
            _command = command;
            _command.AddToken("SELECT");
            _command.AddToken(s);
        }

        public From From(string tableName)
        {
            return new From(_command, tableName);
            
        }
    }

    public class From
    {
        private readonly SqlCommand _command;

        public From(SqlCommand command, string tableName)
        {
            _command = command;
            _command.AddToken("FROM");
            _command.AddParameter(tableName, tableName);
        }

        public SqlOuterJoin Outer
        {
            get
            {
                return new SqlOuterJoin(_command);
            }
            
        }

        public Where Where(string fildName)
        {
            return new Where(_command, fildName);
        }

        public SqlJoin Join(string table2)
        {
            return new SqlJoin(_command, table2.Trim());

        }
    }

    public class SqlOuterJoin
    {
        private readonly SqlCommand _command;

        public SqlOuterJoin(SqlCommand command)
        {
            _command = command;
        }

        public SqlJoin Join(string name)
        {
            return new SqlJoin(_command);
        }
    }

    public class SqlJoin
    {
        private readonly SqlCommand _command;

        public SqlJoin(SqlCommand command, string table2)
        {
            _command = command;
            _command.AddToken("JOIN");
            _command.AddParameter(table2, table2);
        }

        public SqlJoin(SqlCommand command)
        {
            _command = command;
        }

        public SqlJoin2 On(string fildName)
        {
            _command.AddToken("ON");
            _command.AddToken(fildName);
            return new SqlJoin2(_command);
        }}

    public class SqlJoin2
    {
        private readonly SqlCommand _command;

        public SqlJoin2(SqlCommand command)
        {
            _command = command;
        }

        public SqlJoin3 Eq<T>(T value)
        {
            _command.AddToken("=");
            _command.AddParameter(value);
            return new SqlJoin3(_command);
        }
    }

    public class SqlJoin3
    {
        private readonly SqlCommand _command;
        public SqlJoin3(SqlCommand command)
        {
            _command = command;
        }

        public SqlJoin2 And(string fildName)
        {
            _command.AddToken("AND");
            _command.AddToken(fildName);
            return new SqlJoin2(_command);
        }

        public Where Where(string name)
        {
            return new Where(_command, name);
        }

        public SqlJoin Join(string empty)
        {
            return new SqlJoin(_command, empty);
        }
    }


    public class Where
    {
        private readonly SqlCommand _command;

        public Where(SqlCommand command, string fildName)
        {
            _command = command;
            _command.AddToken("WHERE");
            _command.AddToken(fildName);
        }

        public Where And(string fildName)
        {
            _command.AddToken("AND");
            _command.AddToken(fildName);
            return this;
        }

        public Where Eq<T>(T value)
        {
            _command.AddToken("=");
            _command.AddParameter(value);
            return this;
        }
    }
}