﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Swashbuckle.AspNetCore.Annotations.Test;

public class AnnotationsSchemaFilterTests
{
    [Theory]
    [InlineData(typeof(SwaggerAnnotatedType))]
    [InlineData(typeof(SwaggerAnnotatedStruct))]
    public void Apply_EnrichesSchemaMetadata_IfTypeDecoratedWithSwaggerSchemaAttribute(Type type)
    {
        var schema = new OpenApiSchema();
        var context = new SchemaFilterContext(type: type, schemaGenerator: null, schemaRepository: null);

        Subject().Apply(schema, context);

        Assert.Equal($"Description for {type.Name}", schema.Description);
        Assert.Equal(["StringWithSwaggerSchemaAttribute"], schema.Required);
        Assert.Equal($"Title for {type.Name}", schema.Title);
    }

    [Fact]
    public void Apply_EnrichesSchemaMetadata_IfParameterDecoratedWithSwaggerSchemaAttribute()
    {
        var schema = new OpenApiSchema();
        var parameterInfo = typeof(FakeControllerWithSwaggerAnnotations)
            .GetMethod(nameof(FakeControllerWithSwaggerAnnotations.ActionWithSwaggerSchemaAttribute))
            .GetParameters()[0];
        var context = new SchemaFilterContext(
            type: parameterInfo.ParameterType,
            schemaGenerator: null,
            schemaRepository: null,
            parameterInfo: parameterInfo);

        Subject().Apply(schema, context);

        Assert.Equal($"Description for param", schema.Description);
        Assert.Equal("date", schema.Format);
    }

    [Theory]
    [InlineData(typeof(SwaggerAnnotatedType), nameof(SwaggerAnnotatedType.StringWithSwaggerSchemaAttribute), true, true, false)]
    [InlineData(typeof(SwaggerAnnotatedStruct), nameof(SwaggerAnnotatedStruct.StringWithSwaggerSchemaAttribute), true, true, false)]
    public void Apply_EnrichesSchemaMetadata_IfPropertyDecoratedWithSwaggerSchemaAttribute(
        Type declaringType,
        string propertyName,
        bool expectedReadOnly,
        bool expectedWriteOnly,
        bool expectedNullable)
    {
        var schema = new OpenApiSchema { Nullable = true };
        var propertyInfo = declaringType
            .GetProperty(propertyName);
        var context = new SchemaFilterContext(
            type: propertyInfo.PropertyType,
            schemaGenerator: null,
            schemaRepository: null,
            memberInfo: propertyInfo);

        Subject().Apply(schema, context);

        Assert.Equal($"Description for {propertyName}", schema.Description);
        Assert.Equal("date", schema.Format);
        Assert.Equal(expectedReadOnly, schema.ReadOnly);
        Assert.Equal(expectedWriteOnly, schema.WriteOnly);
        Assert.Equal(expectedNullable, schema.Nullable);
    }

    [Fact]
    public void Apply_DoesNotModifyFlags_IfNotSpecifiedWithSwaggerSchemaAttribute()
    {
        var schema = new OpenApiSchema { ReadOnly = true, WriteOnly = true, Nullable = true };
        var propertyInfo = typeof(SwaggerAnnotatedType)
            .GetProperty(nameof(SwaggerAnnotatedType.StringWithSwaggerSchemaAttributeDescriptionOnly));
        var context = new SchemaFilterContext(
            type: propertyInfo.PropertyType,
            schemaGenerator: null,
            schemaRepository: null,
            memberInfo: propertyInfo);

        Subject().Apply(schema, context);

        Assert.True(schema.ReadOnly);
        Assert.True(schema.WriteOnly);
        Assert.True(schema.Nullable);
    }

    [Theory]
    [InlineData(typeof(SwaggerAnnotatedType))]
    [InlineData(typeof(SwaggerAnnotatedStruct))]
    public void Apply_DelegatesToSpecifiedFilter_IfTypeDecoratedWithFilterAttribute(Type type)
    {
        var schema = new OpenApiSchema();
        var context = new SchemaFilterContext(type: type, schemaGenerator: null, schemaRepository: null);

        Subject().Apply(schema, context);

        Assert.NotEmpty(schema.Extensions);
    }

    private static AnnotationsSchemaFilter Subject()
    {
        // A service provider is required from .NET 8 onwards.
        // See https://learn.microsoft.com/dotnet/core/compatibility/extensions/8.0/activatorutilities-createinstance-null-provider.
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        return new AnnotationsSchemaFilter(serviceProvider);
    }
}
