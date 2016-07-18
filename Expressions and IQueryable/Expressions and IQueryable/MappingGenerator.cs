using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionsAndIQueryable
{
    public class MappingGenerator
    {
        public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
        {
            var sourceParam = Expression.Parameter(typeof(TSource));

            var propertiesMapping = GetPropertiesMappings<TDestination>(sourceParam);
            var fieldsMapping = GetFieldsMappings<TDestination>(sourceParam);

            var mapFunction =
                Expression.Lambda<Func<TSource, TDestination>>(
                    Expression.MemberInit(
                        Expression.New(typeof(TDestination)),
                        propertiesMapping.Union(fieldsMapping)
                    ),
                    sourceParam
                );

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }

        private IEnumerable<MemberAssignment> GetPropertiesMappings<TDestination>(ParameterExpression sourceParam)
        {
            var sourceProperties = sourceParam.Type.GetProperties();

            var propertyToPropertyMappings = sourceProperties
                                                .Where(p => typeof(TDestination).GetProperty(p.Name)?.PropertyType == p.PropertyType)
                                                .Select(p => Expression.Bind(
                                                    typeof(TDestination).GetProperty(p.Name),
                                                    Expression.Property(sourceParam, p)));

            var propertyToFieldMappings = sourceProperties
                                                .Where(p => typeof(TDestination).GetField(p.Name)?.FieldType == p.PropertyType)
                                                .Select(p => Expression.Bind(
                                                    typeof(TDestination).GetField(p.Name),
                                                    Expression.Property(sourceParam, p)));

            return propertyToPropertyMappings.Union(propertyToFieldMappings);
        }

        private IEnumerable<MemberAssignment> GetFieldsMappings<TDestination>(ParameterExpression sourceParam)
        {
            var sourceFields = sourceParam.Type.GetFields();

            var fieldToFieldMappings = sourceFields
                                                .Where(f => typeof(TDestination).GetField(f.Name)?.FieldType == f.FieldType)
                                                .Select(f => Expression.Bind(
                                                    typeof(TDestination).GetField(f.Name),
                                                    Expression.Field(sourceParam, f)));

            var fieldToPropertyMappings = sourceFields
                                                .Where(f => typeof(TDestination).GetProperty(f.Name)?.PropertyType == f.FieldType)
                                                .Select(f => Expression.Bind(
                                                    typeof(TDestination).GetProperty(f.Name),
                                                    Expression.Field(sourceParam, f)));

            return fieldToFieldMappings.Union(fieldToPropertyMappings);
        }
    }
}
