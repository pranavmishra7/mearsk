using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
	public class InMemoryDbTestBase
	{
		private string _dbName;
		protected string DbName => _dbName ??= $"MearskDb";
		protected MearskContext Context { get; set; }

		public InMemoryDbTestBase()
		{
			Context = GetInMemoryContext();
		}

		protected MearskContext GetInMemoryContext()
		{
			var options = new DbContextOptionsBuilder<MearskContext>()
				.UseInMemoryDatabase(databaseName: DbName)
				.EnableSensitiveDataLogging(true)
				.Options;
			return new MearskContext(options);
		}
	}
}
