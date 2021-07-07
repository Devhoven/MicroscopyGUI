//*************************************************************
// Copyright (c) 1991-2020 LEAD Technologies, Inc.
// All Rights Reserved.
//*************************************************************

#if !defined(LTVER_H)
#define LTVER_H

#if defined(LTV15_CONFIG)
#define LTVER_   1500
#define L_VER_DESIGNATOR "15"
#define L_VER_DESIGNATOR_STR "15"
#define FOR_PRE_16_5
#elif defined(LTV16_CONFIG)
#define LTVER_   1600
#define L_VER_DESIGNATOR "16"
#define L_VER_DESIGNATOR_STR "16"
#elif defined(LTV17_CONFIG)
#define LTVER_   1700
#define L_VER_DESIGNATOR "17"
#define L_VER_DESIGNATOR_STR "17"
#elif defined(LTV175_CONFIG)
#define LTVER_   1750
#define L_VER_DESIGNATOR "175"
#define L_VER_DESIGNATOR_STR "17.5"
#elif defined(LTV18_CONFIG)
#define LTVER_   1800
#define L_VER_DESIGNATOR "18"
#define L_VER_DESIGNATOR_STR "18"
#elif defined(LTV19_CONFIG)
#define LTVER_   1900
#define L_VER_DESIGNATOR "19"
#define L_VER_DESIGNATOR_STR "19"
#elif defined(LTV20_CONFIG)
#define LTVER_   2000
#define L_VER_DESIGNATOR "20"
#define L_VER_DESIGNATOR_STR "20"
#elif defined(LTV21_CONFIG)
#define LTVER_   2100
#define L_VER_DESIGNATOR "21"
#define L_VER_DESIGNATOR_STR "21"
#else
// You must define LTV##_CONFIG before including any LEADTOOLS header files
// For example:
// #define LTV21_CONFIG    // Using LEADTOOLS v21
// or
// #define LTV175_CONFIG   // Using LEADTOOLS v17.5
#if !defined(RC_INVOKED)
#error LEADTOOLS Vxx_CONFIG not found!
#endif // #if !defined(RC_INVOKED)
#endif // #if defined(LTV15_CONFIG)


#if LTVER_ >= 1600
#define LEADTOOLS_V16_OR_LATER
#endif

#if LTVER_ >= 1700
#define LEADTOOLS_V17_OR_LATER
#endif

#if LTVER_ >= 1750
#define LEADTOOLS_V175_OR_LATER
#endif

#if LTVER_ >= 1800
#define LEADTOOLS_V18_OR_LATER
#endif

#if LTVER_ >= 1900
#define LEADTOOLS_V19_OR_LATER
#endif

#if LTVER_ >= 2000
#define LEADTOOLS_V20_OR_LATER
#endif

#if LTVER_ >= 2100
#define LEADTOOLS_V21_OR_LATER
#endif

#if defined(FOR_UWP)
   #define L_PLATFORM_DESIGNATOR "UWPUnmanaged"
#elif defined(FOR_WINRT_PHONE)
   #if (_MSC_VER >= 1800)
      #define L_PLATFORM_DESIGNATOR "WinRTPhone8_1"
   #else
      #define L_PLATFORM_DESIGNATOR "WinRTPhone"
   #endif
#elif defined(FOR_WINRT)
   #if (_MSC_VER >= 1800)
      #define L_PLATFORM_DESIGNATOR "WinRT8_1"
   #else
      #define L_PLATFORM_DESIGNATOR "WinRT"
   #endif
#else
   #if defined(LEADTOOLS_V20_OR_LATER)
      #define L_PLATFORM_DESIGNATOR "CDLL"
   #else
      #if (_MSC_VER >= 1910)     /* VS 2017 - VC15*/
         #define L_PLATFORM_DESIGNATOR "CDLLVC15"
      #elif (_MSC_VER >= 1900)     /* VS 2015 - VC14*/
         #define L_PLATFORM_DESIGNATOR "CDLLVC14"
      #elif (_MSC_VER >= 1800)   /* VS 2013 - VC12*/
         #define L_PLATFORM_DESIGNATOR "CDLLVC12"
      #elif (_MSC_VER >= 1700)   /* VS 2012 - VC11*/
         #define L_PLATFORM_DESIGNATOR "CDLLVC11"
      #elif (_MSC_VER >= 1600)   /* VS 2010 - VC10*/
         #define L_PLATFORM_DESIGNATOR "CDLLVC10"
      #else                      /* VS 2008 - VC09*/
         #define L_PLATFORM_DESIGNATOR "CDLL"
      #endif
   #endif
#endif

#ifdef FOR_DEBUG
#define L_DEBUG_SUBFOLDER  "\\Debug"
#else
#define L_DEBUG_SUBFOLDER  ""
#endif // #ifdef FOR_DEBUG

#if defined(FOR_WIN64)
#define L_LIB_SUBFOLDER  "x64"
#define L_LIB_SUFFIX "x"
#elif defined(FOR_WIN32)
#define L_LIB_SUBFOLDER  "Win32"
#define L_LIB_SUFFIX "u"
#elif defined(FOR_WINCE)
#define L_LIB_SUBFOLDER  "Mobile6"
#define L_LIB_SUFFIX "u"
#endif // #if defined(FOR_WIN64)

#define L_BIN_SUBFOLDER L_LIB_SUBFOLDER L_DEBUG_SUBFOLDER

#ifdef FOR_DOTNET4
#define L_DOTNET_FOLDER "Dotnet4"
#else
#define L_DOTNET_FOLDER "Dotnet"
#endif // #ifdef FOR_DOTNET4

#endif // #if !defined(LTVER_H)
