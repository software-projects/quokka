#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using NUnit.Framework;
using Quokka.Util;

namespace Quokka.Util
{
	[TestFixture]
	public class RegistryUtilTests
	{
		[Test]
		public void KeyPath()
		{
			RegistryUtil.CompanyName = "Company";
			RegistryUtil.ProductName = "Product";
			RegistryUtil.MajorVersion = "1";

			Assert.AreEqual(@"Software\Company\Product\1", RegistryUtil.KeyPath);

			RegistryUtil.ProductName = null;

			Assert.AreEqual(@"Software\Company\1", RegistryUtil.KeyPath);

			RegistryUtil.CompanyName = "ABC Company Name";
			RegistryUtil.ProductName = null;

			Assert.AreEqual(@"Software\ABC Company Name\1", RegistryUtil.KeyPath);

			RegistryUtil.ProductName = "Product";
			RegistryUtil.MajorVersion = null;

			Assert.AreEqual(@"Software\ABC Company Name\Product", RegistryUtil.KeyPath);
		}

		[Test]
		public void SubKeyPath()
		{
			RegistryUtil.CompanyName = "Company";
			RegistryUtil.ProductName = "Product";
			RegistryUtil.MajorVersion = "1";

			Assert.AreEqual(@"Software\Company\Product\1\SubKey", RegistryUtil.SubKeyPath("SubKey"));
			Assert.AreEqual(@"Software\Company\Product\1\SubKey1\SubKey2", RegistryUtil.SubKeyPath("SubKey1", "SubKey2"));

			Assert.AreEqual(@"Software\Company\Product\1", RegistryUtil.SubKeyPath((string)null));
			Assert.AreEqual(@"Software\Company\Product\1\SubKey1\SubKey2", RegistryUtil.SubKeyPath("SubKey1", null, "SubKey2"));

		}

		[Test]
		public void ThrowsExceptionIfCompanyOrProductNotDefined()
		{
			RegistryUtil.CompanyName = null;
			RegistryUtil.ProductName = null;
			string s;
			Assert.Throws<QuokkaException>(() => s = RegistryUtil.KeyPath);
		}
	}
}