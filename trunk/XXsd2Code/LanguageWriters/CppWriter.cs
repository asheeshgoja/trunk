﻿/*
This file is part of XXsd2Code <http://xxsd2code.sourceforge.net/>

XXsd2Code is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

XXsd2Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with XXsd2Code.  If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Schema;

namespace XXsd2Code.LanguageWriters
{
    public class CppWriter : LanguageWriterBase
    {

        public CppWriter(
            List<String> classNamesNoNestedTypes,
            List<String> classNames,
            List<String> includeFiles,
            List<String> includeFilesToSkip,
            List<String> externalNamespaces,
            Dictionary<int, string> xsdnNamespaces,
            string destinationFolder,
            string outerClassName,
            Dictionary<string, List<ClassElement>> externalClassesToGenerateMap,
            Dictionary<string, string> externalClassesnNamespaces,
            Dictionary<string, string> externalEnumsnNamespaces,
            Dictionary<string, List<EnumElement>> externalEnumsToGenerateMap,
            TargetLanguage targetLanguage
        ) 
        {
            _targetLanguage = targetLanguage;
            _classNamesNoNestedTypes = classNamesNoNestedTypes;
            _classNames = classNames;
            _includeFiles = includeFiles;
            _includeFilesToSkip = includeFilesToSkip;
            _externalNamespaces = externalNamespaces;
            _xsdnNamespaces = xsdnNamespaces;
            _destinationFolder = destinationFolder;
            _outerClassName = outerClassName;
            _externalClassesToGenerateMap = externalClassesToGenerateMap;
            _externalClassesnNamespaces = externalClassesnNamespaces;
            _externalEnumsnNamespaces = externalEnumsnNamespaces;
            _externalEnumsToGenerateMap = externalEnumsToGenerateMap;
        }

        public override void Write(StreamWriter sw, string namespaceName, Dictionary<string,
                                                        List<EnumElement>> enumsToGenerateMap,
                                                        Dictionary<string, List<ClassElement>> classesToGenerateMap)
        {
            sw.WriteLine("//Auto generated code");
            FileVersionInfo fv = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            sw.WriteLine("//Code generated by XXsd2Code<http://xxsd2code.sourceforge.net/> {0} version {1}", fv.InternalName, fv.FileVersion, System.DateTime.Now.ToString());
            sw.WriteLine("//For any comments/suggestions contact code generator author at asheesh.goja@gmail.com");
            sw.WriteLine("//Auto generated code");
            sw.WriteLine("//Auto generated code");
            sw.WriteLine(" ");
            sw.WriteLine(" ");
            sw.WriteLine(" ");

            sw.WriteLine("#pragma once");
            sw.WriteLine(" ");
            sw.WriteLine(" ");
            //sw.WriteLine("#if _MSC_VER > 1300");
            //sw.WriteLine("#pragma managed(push, off)");
            //sw.WriteLine("#endif // _MSC_VER > 1300");
            sw.WriteLine(" ");
            sw.WriteLine(" ");

            if(CrossPlatformSerializationSupport)
                sw.WriteLine("#include \"SerailizerBase.h\"");


            sw.WriteLine("#include <string>");
            sw.WriteLine("#include <vector>");
            sw.WriteLine("#include <functional>");
            sw.WriteLine("#include <algorithm>");
            sw.WriteLine("using namespace std;");
            sw.WriteLine("#include <tchar.h>");
            sw.WriteLine("typedef basic_string<TCHAR> tstring;");
            sw.WriteLine(" ");
            sw.WriteLine(" ");

            foreach (string f in _includeFiles)
            {
                FileInfo fi = new FileInfo(f);
                sw.WriteLine("#include \"{0}\"", fi.Name);
            }

            sw.WriteLine(" ");
            sw.WriteLine(" ");
            foreach (string s in _externalNamespaces)
            {
                string nS = s;

                //if(MixedMode)
                //    nS = s.Replace("DataContract", "Contracts");

                sw.WriteLine("using namespace {0};", nS);
            }

            //sw.WriteLine("using namespace ::Serializers;");

            sw.WriteLine(" ");
            sw.WriteLine(" ");

            //String[] namespaceParts = new String[] { namespaceName, "Contracts" };
            //if(MixedMode)
            //    namespaceName = namespaceName.Replace("DataContract", "Contracts");

            String[] namespaceParts = namespaceName.Split(new string[1] { "::" }, StringSplitOptions.None);

            GenerateNamespaceBeginBlock(namespaceParts, sw);
            GenerateEnums(sw, enumsToGenerateMap);
            GenerateClasses(sw, classesToGenerateMap, enumsToGenerateMap);
            GenerateNamespaceEndBlock(namespaceParts, sw);

            sw.WriteLine("");
            sw.WriteLine("");
            //sw.WriteLine("#if _MSC_VER > 1300");
            //sw.WriteLine("#pragma managed(pop)");
            //sw.WriteLine("#endif // _MSC_VER > 1300");

            sw.Close();

        }


        protected override void WriteClass(StreamWriter sw, string className, List<ClassElement> classMetadata,
                   Dictionary<String, List<ClassElement>> classesToGenerateMap,
                   Dictionary<String, List<EnumElement>> enumsToGenerateMap)
        {
            bool containsNestedCollection = false;

            if (IsExternalType(className))
            {
                classesToGenerateMap.Remove(className);
                return;
            }

            List<string> dep = GetClassDependencies(className, classesToGenerateMap);

            foreach (string s in dep)
            {
                if (s == className) continue;

                if (classesToGenerateMap.ContainsKey(s))
                {
                    WriteClass(sw, s, classesToGenerateMap[s], classesToGenerateMap, enumsToGenerateMap);
                    classesToGenerateMap.Remove(s);
                }
            }
            List<ClassElement> val = classMetadata;

            string contextClassName = "";
            contextClassName = String.Format("{0}", className);

            List<string> vars = new List<string>();

            sw.WriteLine(" ");
            //sw.WriteLine("{0}class\tDATA_CONTRACT_API\t{1}", GetTab(), contextClassName);
            sw.WriteLine("{0}class\t{1}", GetTab(), contextClassName);
            sw.WriteLine("{0}{1}", GetTab(), "{");
            sw.WriteLine("{0}public:", GetTab());


            #region Declarations

            IndentLevel++;
            String collectionType = String.Empty;
            foreach (ClassElement var in val)
            {
                if (var.IsCollection)
                {
                    //collectionType = String.Format("{0}_VECTOR", XSDToCppType(var));
                    collectionType = CreateFormattedVectorDeclaration(var);

                    if (var.CustomType == null)
                    {
                        sw.WriteLine("");
                        sw.WriteLine("{0}//Explicit Template Instantiation  Takes care of Warning C4251", GetTab());
                        //sw.WriteLine("{0}template class DATA_CONTRACT_API std::allocator<{1}>;", GetTab(), XSDToCppType(var));
                        sw.WriteLine("{0}template class std::allocator<{1}>;", GetTab(), XSDToCppType(var));
                        sw.WriteLine("{0}//End Explicit Template Instantiation ", GetTab());
                        sw.WriteLine("");
                        //sw.WriteLine("{0}template class DATA_CONTRACT_API std::vector<{1}, std::allocator<{1}> >;", GetTab(), XSDToCppType(var));
                        sw.WriteLine("{0}template class std::vector<{1}, std::allocator<{1}> >;", GetTab(), XSDToCppType(var));
                        sw.WriteLine(GetTab() + "typedef vector<" + XSDToCppType(var) + ">\t\t" + collectionType + ";");
                    }
                    else
                    {
                        sw.WriteLine("");
                        sw.WriteLine("{0}//Explicit Template Instantiation  Takes care of Warning C4251", GetTab());
                        //sw.WriteLine("{0}template class DATA_CONTRACT_API std::allocator<{1}*>;", GetTab(), XSDToCppType(var));
                        //sw.WriteLine("{0}template class DATA_CONTRACT_API std::vector<{1}*, std::allocator<{1}*> >;", GetTab(), XSDToCppType(var));
                        sw.WriteLine("{0}template class  std::allocator<{1}*>;", GetTab(), XSDToCppType(var));
                        sw.WriteLine("{0}template class  std::vector<{1}*, std::allocator<{1}*> >;", GetTab(), XSDToCppType(var));
                        sw.WriteLine("{0}//End Explicit Template Instantiation ", GetTab());
                        sw.WriteLine("");
                        sw.WriteLine(GetTab() + "typedef vector<" + XSDToCppType(var) + "*>\t\t" + collectionType + ";");
                    }

                    sw.WriteLine("{0}{1}{0}{2};{3}", GetTab(), collectionType, var.Name, var.Comment);
                }
                else
                {
                    string nSpace = string.Empty;
                    //if (MixedMode && (var.CustomType != null))
                    //{
                    //    if (_externalClassesToGenerateMap.ContainsKey(var.CustomType))
                    //        nSpace = _externalClassesToGenerateMap[var.CustomType][0].Namespace + "::";
                    //    else if (_externalEnumsToGenerateMap.ContainsKey(var.CustomType))
                    //    {
                    //        nSpace = _externalEnumsnNamespaces[var.CustomType] + "::";
                    //    }
                    //    else
                    //        nSpace = "";// var.Namespace + "::";
                    //}
                    sw.WriteLine("{0}{4}{1}{0}{2};{3}", GetTab(), XSDToCppType(var), var.Name, var.Comment, nSpace);
                }
            }

            #endregion


            #region Default constructor
            sw.WriteLine("			");
            sw.WriteLine(GetTab() + "//default constructor");
            String defaultCtor = String.Format("{0}{1}()", GetTab(), contextClassName);
            sw.WriteLine(defaultCtor);
            sw.WriteLine(GetTab() + "{");
            IndentLevel++;
            foreach (ClassElement var in val)
            {
                //if (var.Type == XmlTypeCode.Boolean)
                //{
                string defVal = GetDefaultsString(var, enumsToGenerateMap);
                if (defVal != "")
                {
                    String defaultVal = String.Format("{0}{1} = {2} ;", GetTab(), var.Name, defVal);
                    sw.WriteLine(defaultVal);
                }
                if (var.IsEnum)
                {
                }
                //}
            }
            IndentLevel--;
            sw.WriteLine(GetTab() + "}");
            #endregion


            #region Destructor
            sw.WriteLine("			");
            sw.WriteLine(GetTab() + "//Destructor");
            String defaultDtor = String.Format("{0}~{1}()", GetTab(), contextClassName);
            sw.WriteLine(defaultDtor);
            sw.WriteLine(GetTab() + "{");
            IndentLevel++;
            foreach (ClassElement var in val)
            {
                if (var.IsCollection && var.CustomType != null)
                {
                    String vectorType = CreateFormattedVectorDeclaration(var); //String.Format("{0}_VECTOR", XSDToCppType(var));
                    String varName = var.Name;
                    sw.WriteLine("{0}for_each({1}.begin(),{1}.end(),&{2}::DeleteElement<{3}::value_type>);", GetTab(), varName, contextClassName, vectorType);
                    sw.WriteLine("{0}{1}.clear();", GetTab(), varName);
                    containsNestedCollection = true;
                }
            }
            IndentLevel--;
            sw.WriteLine(GetTab() + "}");
            #endregion


            #region Copy constuctor
            sw.WriteLine("			");
            sw.WriteLine(GetTab() + "//copy constuctor");

            String copyCtor = String.Format("{0}{1}(const  {1}& rhs){2}*this = rhs;{3}", GetTab(), contextClassName, "{", "}");
            sw.WriteLine(copyCtor);
            #endregion


            #region = Operator
            sw.WriteLine("			");
            sw.WriteLine(GetTab() + "//= operator");
            String equalToOperator = String.Format("{0}{1}& operator = (const {1}& rhs)", GetTab(), contextClassName);
            sw.WriteLine(equalToOperator);
            sw.WriteLine(GetTab() + "{");
            IndentLevel++;
            foreach (ClassElement var in val)
            {
                if (var.IsCollection && var.CustomType != null)
                {
                    String equalStatement = String.Format("{0}CopyVector<{1}>({2},rhs.{2}) ;", GetTab(), var.CustomType, var.Name);
                    sw.WriteLine(equalStatement);
                }
                else
                {
                    String equalStatement = String.Format("{0}{1} = rhs.{1} ;", GetTab(), var.Name);
                    sw.WriteLine(equalStatement);
                }
            }
            sw.WriteLine(GetTab() + "return *this;");
            IndentLevel--;
            sw.WriteLine(GetTab() + "}");
            #endregion

            #region Metadata 
            if (CrossPlatformSerializationSupport)
            {
                sw.WriteLine("			");
                sw.WriteLine(GetTab() + "//Metadata");

                sw.WriteLine(GetTab() + "METADATA_BEGIN");

                IndentLevel++;
                foreach (ClassElement var in val)
                {
                    if (var.IsCollection)
                    {
                        collectionType = CreateFormattedVectorDeclaration(var); //String.Format("{0}_VECTOR", XSDToCppType(var));
                        if (var.CustomType == null)
                            sw.WriteLine(GetTab() + "ADD_MEMBER_COLLECTION_LITE(" + collectionType + "," + var.Name + ")");
                        else
                            sw.WriteLine(GetTab() + "ADD_MEMBER_COLLECTION_NESTED(" + collectionType + "," + var.Name + ")");
                    }
                    else if (var.IsEnum)
                    {
                        sw.WriteLine(GetTab() + "ADD_MEMBER_ENUM(" + var.CustomType + "," + var.Name + ")");
                    }
                    else
                    {
                        if (var.Type == XmlTypeCode.Element)
                            sw.WriteLine(GetTab() + "ADD_MEMBER_NESTED(" + var.Name + ")");
                        else
                            sw.WriteLine(GetTab() + "ADD_MEMBER(" + var.Name + ")");
                    }
                    vars.Add(var.Name);
                }
                IndentLevel--;
                sw.WriteLine(GetTab() + "METADATA_END");
                IndentLevel--;
            }
            #endregion


            #region Delete/Copy vector contents
            if (containsNestedCollection)
            {
                sw.WriteLine("			");
                sw.WriteLine("			");
                sw.WriteLine(GetTab() + "//Collection helpers");
                sw.WriteLine("{0}private:", GetTab());
                IndentLevel++;
                sw.WriteLine(GetTab() + "template<typename TYPE>");
                sw.WriteLine(GetTab() + "static void DeleteElement(TYPE element){delete element;}");

                sw.WriteLine("			");
                sw.WriteLine(GetTab() + "template<typename T , typename VECTOR>");
                sw.WriteLine(GetTab() + "void CopyVector(VECTOR& dst, const  VECTOR& src )");
                sw.WriteLine(GetTab() + "{");
                IndentLevel++;
                sw.WriteLine(GetTab() + "for_each(dst.begin(),dst.end(),&{0}::DeleteElement<VECTOR::value_type>);", className);
                sw.WriteLine(GetTab() + "dst.clear();");
                sw.WriteLine(GetTab() + "for(size_t i = 0 ; i < src.size() ; i++)	dst.push_back( new T( *(src[i]) ) ) ;");
                IndentLevel--;
                sw.WriteLine(GetTab() + "}");
                IndentLevel--;
            }
            #endregion

            sw.WriteLine("			");
            sw.WriteLine(GetTab() + "};");

            classesToGenerateMap.Remove(className);
        }

    }
}
