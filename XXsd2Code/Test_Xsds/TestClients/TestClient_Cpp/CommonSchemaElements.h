//Auto generated code
//Code generated by XXsd2Code<http://xxsd2code.sourceforge.net/> XXsd2Code.exe version 1.0.0
//For any comments/suggestions contact code generator author at asheesh.goja@gmail.com
//Auto generated code
//Auto generated code
 
 
 
#pragma once
 
 
 
 
#include <string>
#include <vector>
#include <functional>
#include <algorithm>
using namespace std;
#include <tchar.h>
typedef basic_string<TCHAR> tstring;
 
 
 
 
 
 
namespace XXsd2CodeSample
{
	namespace CommonElements
	{
 
		//enumeration	FuzzyCondition
		enum	FuzzyCondition
		{
			Hot = 100,
			VeryHot = 200,
			ExtremelyHot = 300,
			Cold = 0
			
		};
 
		class	CreditCardDetails
		{
		public:
			tstring			CCNumber;
			tstring			ExpirationDate;
			FuzzyCondition			Rating;
			
			//default constructor
			CreditCardDetails()
			{
				Rating = XXsd2CodeSample::CommonElements::Hot ;
			}
			
			//Destructor
			~CreditCardDetails()
			{
			}
			
			//copy constuctor
			CreditCardDetails(const  CreditCardDetails& rhs){*this = rhs;}
			
			//= operator
			CreditCardDetails& operator = (const CreditCardDetails& rhs)
			{
				CCNumber = rhs.CCNumber ;
				ExpirationDate = rhs.ExpirationDate ;
				Rating = rhs.Rating ;
				return *this;
			}
			
			};
 
			class	OrderItem
			{
			public:
				bool				IsBackOrder;
				tstring				title;
				tstring				note;
				int				quantity;
				double				price;
			
				//default constructor
				OrderItem()
				{
					IsBackOrder = false ;
					quantity = 0 ;
					price = 0.0 ;
				}
			
				//Destructor
				~OrderItem()
				{
				}
			
				//copy constuctor
				OrderItem(const  OrderItem& rhs){*this = rhs;}
			
				//= operator
				OrderItem& operator = (const OrderItem& rhs)
				{
					IsBackOrder = rhs.IsBackOrder ;
					title = rhs.title ;
					note = rhs.note ;
					quantity = rhs.quantity ;
					price = rhs.price ;
					return *this;
				}
			
				};
			}
		}


