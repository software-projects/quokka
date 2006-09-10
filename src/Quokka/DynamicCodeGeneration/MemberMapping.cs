using System;
using System.Collections.Generic;
using System.Reflection;

namespace Quokka.DynamicCodeGeneration
{
    /// <summary>
    /// The idea of this class is to encapsulate the logic of mapping members for the
    /// purpose of 'Duck Typing'.
    /// </summary>
    public class MemberMapping
    {
        public MemberMapping(Type interfaceType, Type innerType) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Can the mapping be used to generate a new type
        /// </summary>
        public bool IsValid {
            get {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When <see cref="IsValid"/> is <c>false</c>, contains an error message indicating why
        /// </summary>
        public string ErrorMessage {
            get {
                throw new NotImplementedException();
            }
        }

        public IList<MemberInfo> MissingMembers {
            get {
                throw new NotImplementedException();
            }
        }


    }
}
