//Auto generated code
//Code generated by XXsd2Code<http://xxsd2code.sourceforge.net/> XXsd2Code.exe version 1.0.0
//For any comments/suggestions contact code generator author at asheesh.goja@gmail.com
//Auto generated code
//Auto generated code
 
#pragma once
using System::String;
using System::ICloneable;
using System::Collections::Generic::List;
using System::SerializableAttribute;
 
 
#include "CommonSchemaElements.h"
 
 
using namespace XXsd2CodeSample::CommonElements;
 
 
namespace XXsd2CodeSample
{
 
	[SerializableAttribute]
	public ref class	Address: public ICloneable
	{
	public:
		String^		name;
		int		zip;
		String^		city;
		String^		country;
			
		//default constructor
		Address()
		{
			name = String::Empty;
			zip = 0 ;
			city = String::Empty;
			country = String::Empty;
		}
			
		//copy constuctor
		Address( Address%  rhs){*this = rhs;}
			
		//IClonable Override
		virtual	Object^ Clone()
		{
			Address^	 instance = gcnew Address() ;
			*instance = *this ;
			return instance;
		}
			
		//= operator
		Address% operator = ( Address%  rhs)
		{
			name = rhs.name ;
			zip = rhs.zip ;
			city = rhs.city ;
			country = rhs.country ;
			return *this;
		}
			
		//DeepCopy
		void	DeepCopy ( Address^  from)
		{
			name = from->name ;
			zip = from->zip ;
			city = from->city ;
			country = from->country ;
		}
			
	};
 
	[SerializableAttribute]
	public ref class	CustomerOrder: public ICloneable
	{
	public:
		String^		CustomerID;
		Address^		AddressInfo;

		typedef List<XXsd2CodeSample::CommonElements::OrderItem^>		OrderItem_VECTOR;
		OrderItem_VECTOR^		Orders;
			
		//default constructor
		CustomerOrder()
		{
			CustomerID = String::Empty;
			AddressInfo = gcnew Address() ;
			Orders = gcnew OrderItem_VECTOR() ;
		}
			
		//copy constuctor
		CustomerOrder( CustomerOrder%  rhs){*this = rhs;}
			
		//IClonable Override
		virtual	Object^ Clone()
		{
			CustomerOrder^	 instance = gcnew CustomerOrder() ;
			*instance = *this ;
			return instance;
		}
			
		//= operator
		CustomerOrder% operator = ( CustomerOrder%  rhs)
		{
			CustomerID = rhs.CustomerID ;
			AddressInfo = safe_cast<Address^>(rhs.AddressInfo->Clone()) ;
			Orders->AddRange(rhs.Orders) ;
			return *this;
		}
			
		//DeepCopy
		void	DeepCopy ( CustomerOrder^  from)
		{
			CustomerID = from->CustomerID ;
			AddressInfo->DeepCopy(from->AddressInfo) ;
			Orders->AddRange(from->Orders) ;
		}
			
	};
}


