using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;

namespace MemoryFilter.Tests.Extensions {

    public static class SubstituteExtensions {
        public static T Set<T, TValue>(this T instance, Func<T, TValue> func, TValue value) {
            func(instance).Returns(value);
            return instance;
        }

        public static T SetTrue<T>(this T instance, Func<T, bool> func) {
            func(instance).Returns(true);
            return instance;
        }

        public static T SetFalse<T>(this T instance, Func<T, bool> func) {
            func(instance).Returns(false);
            return instance;
        }
    }

    public static class FluentAssertionsExtensions {
        public static void ShouldForAllBe<T, TProperty>(this IEnumerable<T> collection, Func<T, TProperty> func,
            TProperty value) {
            foreach (var item in collection) {
                func(item).Should().Be(value);
            }
        }
    }

}