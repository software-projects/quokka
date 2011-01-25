#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Quokka.Diagnostics;
using Quokka.DynamicCodeGeneration;
using SqlQuery;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// 	Builds subtypes of <see cref = "DataRecordConverter" /> using System.Reflection.Emit.
	/// </summary>
	internal class DataRecordConverterTypeBuilder
	{
		private readonly List<string> _errors = new List<string>();

		private readonly Dictionary<DataRecordFieldInfo, PropertyInfo> _mapFieldToProperty =
			new Dictionary<DataRecordFieldInfo, PropertyInfo>();

		private readonly Dictionary<PropertyInfo, DataRecordFieldInfo> _mapPropertyToField =
			new Dictionary<PropertyInfo, DataRecordFieldInfo>();

		private readonly DataRecordConverterSpec _queryInfo;

		public DataRecordConverterTypeBuilder(DataRecordConverterSpec queryInfo)
		{
			_queryInfo = Verify.ArgumentNotNull(queryInfo, "queryInfo");

			PropertyInfo[] properties = queryInfo.RecordType.GetProperties();

			var propertyMap = new NameDictionary<PropertyInfo>();
			var dataRecordMap = new NameDictionary<DataRecordFieldInfo>();

			foreach (DataRecordFieldInfo field in queryInfo.Fields)
			{
				dataRecordMap.Add(field.FieldName, field);
			}

			foreach (PropertyInfo property in properties)
			{
				if (property.CanWrite)
				{
					propertyMap.Add(property.Name, property);
				}
			}

			foreach (DataRecordFieldInfo field in queryInfo.Fields)
			{
				PropertyInfo property;
				if (propertyMap.TryGetValue(field.FieldName, out property))
				{
					if (DataRecordConverterMethod.CanHandleConversion(field.FieldType, property.PropertyType))
					{
						_mapFieldToProperty.Add(field, property);
					}
					else
					{
						_errors.Add(string.Format("Cannot convert from {0} to {1} for {2}",
						                          field.FieldType.Name, property.PropertyType.Name, field.FieldName));
					}
				}
				else
				{
					_errors.Add(string.Format("Type {0} does not have a property named '{1}'",
					                          _queryInfo.RecordType, field.FieldName));
				}
			}

			foreach (PropertyInfo property in properties)
			{
				if (property.CanWrite)
				{
					DataRecordFieldInfo field;
					if (dataRecordMap.TryGetValue(property.Name, out field))
					{
						_mapPropertyToField.Add(property, field);
					}
					else
					{
						_errors.Add(string.Format("Query result does not have a column named '{0}'",
						                          property.Name));
					}
				}
			}
		}

		public IList<string> Errors
		{
			get { return _errors; }
		}

		public bool CanBuildConverter
		{
			get { return _errors.Count == 0; }
		}

		public Type BuildConverter()
		{
			const TypeAttributes typeAttributes = TypeAttributes.Public |
			                                      TypeAttributes.Class |
			                                      TypeAttributes.AutoClass |
			                                      TypeAttributes.AnsiClass |
			                                      TypeAttributes.BeforeFieldInit |
			                                      TypeAttributes.AutoLayout;

			TypeBuilder typeBuilder = DynamicAssembly.Instance.DefineType("DataRecordConverter", typeAttributes,
			                                                              typeof (DataRecordConverter));

			if (!CanBuildConverter)
			{
				var sb = new StringBuilder("Cannot build converter for ");
				sb.Append(_queryInfo.RecordType.FullName);
				sb.AppendLine(":");
				foreach (string errorMessage in _errors)
				{
					sb.AppendLine(errorMessage);
				}
				throw new InvalidOperationException(sb.ToString());
			}

			BuildConstructor(typeBuilder);
			BuildDoCopyMethod(typeBuilder);
			return typeBuilder.CreateType();
		}

		private static void BuildConstructor(TypeBuilder typeBuilder)
		{
			Type parentType = typeof (DataRecordConverter);
			var parameters = new[] {typeof (IDataReader)};
			ConstructorInfo parentConstructor = parentType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
			                                                              parameters, null);
			const MethodAttributes methodAttributes = MethodAttributes.Public
			                                          | MethodAttributes.SpecialName
			                                          | MethodAttributes.RTSpecialName
			                                          | MethodAttributes.HideBySig;

			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(methodAttributes,
			                                                                      CallingConventions.Standard,
			                                                                      new[] {typeof (IDataReader)});

			ILGenerator generator = constructorBuilder.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Call, parentConstructor);
			generator.Emit(OpCodes.Ret);
		}

		private void BuildDoCopyMethod(TypeBuilder typeBuilder)
		{
			const MethodAttributes methodAttributes = MethodAttributes.Family
			                                          | MethodAttributes.HideBySig
			                                          | MethodAttributes.Virtual;


			MethodBuilder methodBuilder = typeBuilder.DefineMethod("DoCopy", methodAttributes, CallingConventions.Standard,
			                                                       typeof (void), new[] {typeof (object)});

			ILGenerator generator = methodBuilder.GetILGenerator();
			generator.DeclareLocal(_queryInfo.RecordType);

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Castclass, _queryInfo.RecordType);
			generator.Emit(OpCodes.Stloc_0);

			foreach (DataRecordFieldInfo fieldInfo in _queryInfo.Fields)
			{
				PropertyInfo property = _mapFieldToProperty[fieldInfo];
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ldarg_0);

				switch (fieldInfo.Index)
				{
					case 0:
						generator.Emit(OpCodes.Ldc_I4_0);
						break;
					case 1:
						generator.Emit(OpCodes.Ldc_I4_1);
						break;
					case 2:
						generator.Emit(OpCodes.Ldc_I4_2);
						break;
					case 3:
						generator.Emit(OpCodes.Ldc_I4_3);
						break;
					case 4:
						generator.Emit(OpCodes.Ldc_I4_4);
						break;
					case 5:
						generator.Emit(OpCodes.Ldc_I4_5);
						break;
					case 6:
						generator.Emit(OpCodes.Ldc_I4_6);
						break;
					case 7:
						generator.Emit(OpCodes.Ldc_I4_7);
						break;
					case 8:
						generator.Emit(OpCodes.Ldc_I4_8);
						break;
					default:
						generator.Emit(OpCodes.Ldc_I4, fieldInfo.Index);
						break;
				}

				generator.Emit(OpCodes.Call, DataRecordConverterMethod.GetMethod(fieldInfo.FieldType, property.PropertyType));
				generator.Emit(OpCodes.Callvirt, _mapFieldToProperty[fieldInfo].GetSetMethod());
			}

			generator.Emit(OpCodes.Ret);
		}
	}
}