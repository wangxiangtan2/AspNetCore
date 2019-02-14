// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Microsoft.AspNetCore.Components.Tests.Forms
{
    public class FieldIdentifierTest
    {
        [Fact]
        public void CannotUseNullModel()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new FieldIdentifier(null, "somefield"));
            Assert.Equal("model", ex.ParamName);
        }

        [Fact]
        public void CannotUseValueTypeModel()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FieldIdentifier(DateTime.Now, "somefield"));
            Assert.Equal("model", ex.ParamName);
            Assert.StartsWith("The model must be a reference-typed object.", ex.Message);
        }

        [Fact]
        public void CannotUseNullFieldName()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new FieldIdentifier(new object(), null));
            Assert.Equal("fieldName", ex.ParamName);
        }

        [Fact]
        public void CanUseEmptyFieldName()
        {
            var fieldIdentifier = new FieldIdentifier(new object(), string.Empty);
            Assert.Equal(string.Empty, fieldIdentifier.FieldName);
        }

        [Fact]
        public void CanGetModelAndFieldName()
        {
            // Arrange/Act
            var model = new object();
            var fieldIdentifier = new FieldIdentifier(model, "someField");

            // Assert
            Assert.Same(model, fieldIdentifier.Model);
            Assert.Equal("someField", fieldIdentifier.FieldName);
        }

        [Fact]
        public void DistinctModelsProduceDistinctHashCodesAndNonEquality()
        {
            // Arrange
            var fieldIdentifier1 = new FieldIdentifier(new object(), "field");
            var fieldIdentifier2 = new FieldIdentifier(new object(), "field");

            // Act/Assert
            Assert.NotEqual(fieldIdentifier1.GetHashCode(), fieldIdentifier2.GetHashCode());
            Assert.False(fieldIdentifier1.Equals(fieldIdentifier2));
        }

        [Fact]
        public void DistinctFieldNamesProduceDistinctHashCodesAndNonEquality()
        {
            // Arrange
            var model = new object();
            var fieldIdentifier1 = new FieldIdentifier(model, "field1");
            var fieldIdentifier2 = new FieldIdentifier(model, "field2");

            // Act/Assert
            Assert.NotEqual(fieldIdentifier1.GetHashCode(), fieldIdentifier2.GetHashCode());
            Assert.False(fieldIdentifier1.Equals(fieldIdentifier2));
        }

        [Fact]
        public void SameContentsProduceSameHashCodesAndEquality()
        {
            // Arrange
            var model = new object();
            var fieldIdentifier1 = new FieldIdentifier(model, "field");
            var fieldIdentifier2 = new FieldIdentifier(model, "field");

            // Act/Assert
            Assert.Equal(fieldIdentifier1.GetHashCode(), fieldIdentifier2.GetHashCode());
            Assert.True(fieldIdentifier1.Equals(fieldIdentifier2));
        }

        [Fact]
        public void FieldNamesAreCaseSensitive()
        {
            // Arrange
            var model = new object();
            var fieldIdentifierLower = new FieldIdentifier(model, "field");
            var fieldIdentifierPascal = new FieldIdentifier(model, "Field");

            // Act/Assert
            Assert.Equal("field", fieldIdentifierLower.FieldName);
            Assert.Equal("Field", fieldIdentifierPascal.FieldName);
            Assert.NotEqual(fieldIdentifierLower.GetHashCode(), fieldIdentifierPascal.GetHashCode());
            Assert.False(fieldIdentifierLower.Equals(fieldIdentifierPascal));
        }

        [Fact]
        public void CanConstructFromExpression_Property()
        {
            var model = new TestModel();
            var fieldIdentifier = new FieldIdentifier(() => model.StringProperty);
            Assert.Same(model, fieldIdentifier.Model);
            Assert.Equal(nameof(model.StringProperty), fieldIdentifier.FieldName);
        }

        [Fact]
        public void CanConstructFromExpression_Field()
        {
            var model = new TestModel();
            var fieldIdentifier = new FieldIdentifier(() => model.StringField);
            Assert.Same(model, fieldIdentifier.Model);
            Assert.Equal(nameof(model.StringField), fieldIdentifier.FieldName);
        }

        [Fact]
        public void CanConstructFromExpression_WithCastToObject()
        {
            // This case is needed because value types will implicitly be cast to object
            var model = new TestModel();
            var fieldIdentifier = new FieldIdentifier(() => model.IntProperty);
            Assert.Same(model, fieldIdentifier.Model);
            Assert.Equal(nameof(model.IntProperty), fieldIdentifier.FieldName);
        }

        [Fact]
        public void CanConstructFromExpression_MemberOfConstantExpression()
        {
            var fieldIdentifier = new FieldIdentifier(() => StringPropertyOnThisClass);
            Assert.Same(this, fieldIdentifier.Model);
            Assert.Equal(nameof(StringPropertyOnThisClass), fieldIdentifier.FieldName);
        }

        string StringPropertyOnThisClass { get; set; }

        class TestModel
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }

#pragma warning disable 649
            public string StringField;
#pragma warning restore 649
        }
    }
}