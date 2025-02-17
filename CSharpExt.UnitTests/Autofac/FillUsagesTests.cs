﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Noggog.Autofac;
using Noggog.Autofac.Validation;
using Xunit;
using Xunit.Sdk;

namespace CSharpExt.UnitTests.Autofac
{
    public class FillUsagesTests
    {
        class NoCtorClass
        {
        }

        class EmptyCtorClass
        {
            public EmptyCtorClass()
            {
                
            }
        }

        [Fact]
        public void EmptyCtors()
        {
            new GetUsages().Get(
                    typeof(NoCtorClass),
                    typeof(EmptyCtorClass))
                .Should().BeEmpty();
        }

        class SomeParams
        {
            public SomeParams(
                NoCtorClass otherClass,
                SubClass subClass)
            {
            }
        }

        class SubClass
        {
            public SubClass(EmptyCtorClass otherClass)
            {
                
            }
        }

        [Fact]
        public void Typical()
        {
            new GetUsages().Get(
                    typeof(SomeParams))
                .Should().Equal(
                    typeof(NoCtorClass),
                    typeof(SubClass));
        }
    }
}