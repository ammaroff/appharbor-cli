﻿using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests
{
	public class TypeNameMatcherTest
	{
		class FooCommand : IFoo { }
		class FooBarCommand : IFoo { }
		class FooBazCommand : IFoo { }

		private static Type FooCommandType = typeof(FooCommand);
		private static Type FooBarCommandType = typeof(FooBarCommand);
		private static Type FooBazCommandType = typeof(FooBazCommand);

		[Fact]
		public void ShouldThrowIfInitializedWithUnnasignableType()
		{
			var exception = Assert.Throws<ArgumentException>(() => new TypeNameMatcher<IFoo>(new List<Type> { typeof(string) }));
		}

		[Theory]
		[InlineData("Foo")]
		[InlineData("foo")]
		public void ShouldGetTypeStartingWithCommandName(string commandName)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType });
			Assert.Equal(FooCommandType, matcher.GetMatchedType(commandName));
		}

		[Theory]
		[InlineData("foo")]
		[InlineData("Foo")]
		public void ShouldNotThrowWhenCommandNameMatchesCommandCompletely(string commandName)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType, FooBarCommandType });
			Assert.Equal(FooCommandType, matcher.GetMatchedType(commandName));
		}

		[Theory]
		[InlineData("Bar")]
		public void ShouldThrowWhenNoTypesMatches(string commandName)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType });

			var exception = Assert.Throws<ArgumentException>(() => matcher.GetMatchedType(commandName));
			Assert.Equal(string.Format("The command \"{0}\" is invalid.", commandName), exception.Message);
		}

		[Theory]
		[InlineData("bar")]
		[InlineData("Bar")]
		public void ShouldReturnScopedCommand(string scope)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType, FooBarCommandType });
			matcher.GetMatchedType(string.Concat("foo", scope));
		}

		[Theory, AutoCommandData]
		public void ShouldBeSatisfiedTypeIsReturned(Mock<TypeNameMatcher<IFoo>> matcher)
		{
			matcher.Setup(x => x.GetMatchedType(It.IsAny<string>())).Returns(typeof(string));
			Assert.True(matcher.Object.IsSatisfiedBy("foo"));
		}

		[Theory, AutoCommandData]
		public void ShouldNotBeSatisfiedWhenTypeCantBeFound(Mock<TypeNameMatcher<IFoo>> matcher)
		{
			matcher.Setup(x => x.GetMatchedType(It.IsAny<string>())).Throws<ArgumentException>();
			Assert.False(matcher.Object.IsSatisfiedBy("foo"));
		}
	}
}
