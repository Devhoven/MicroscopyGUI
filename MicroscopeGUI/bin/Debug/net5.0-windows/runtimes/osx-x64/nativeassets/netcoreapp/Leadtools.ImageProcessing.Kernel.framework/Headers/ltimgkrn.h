//*************************************************************
// Copyright (c) 1991-2020 LEAD Technologies, Inc.
// All Rights Reserved.
//*************************************************************

#if !defined(LTIMGKRN_H)
#define LTIMGKRN_H

#if defined(LEADTOOLS_V19_OR_LATER)

#include "lttyp.h"
#define L_LTIMGKRN_API LT_EXPORTED

#include "ltkrn.h"
#include "ltprimitives.h"

#define L_HEADER_ENTRY
#include "ltpck.h"

enum L_ImgKrnImageFormat
{
   L_ImgKrnImageFormat_RGB888,
   L_ImgKrnImageFormat_BGR888,
   L_ImgKrnImageFormat_RGB8888,
   L_ImgKrnImageFormat_BGR8888,
   L_ImgKrnImageFormat_YV12,
   L_ImgKrnImageFormat_NV12,
   L_ImgKrnImageFormat_NV21,
   L_ImgKrnImageFormat_YUY2
};
typedef enum L_ImgKrnImageFormat L_ImgKrnImageFormat;

enum L_ImgKrnRotateFlipType
{
   L_ImgKrnRotateFlipType_RotateNoneFlipNone   = 0,
   L_ImgKrnRotateFlipType_RotateNoneFlipX      = 1,
   L_ImgKrnRotateFlipType_RotateNoneFlipY      = 2,
   L_ImgKrnRotateFlipType_RotateNoneFlipXY     = 3,
   L_ImgKrnRotateFlipType_Rotate90FlipNone     = 4,
   L_ImgKrnRotateFlipType_Rotate90FlipX        = 5,
   L_ImgKrnRotateFlipType_Rotate90FlipY        = 6,
   L_ImgKrnRotateFlipType_Rotate90FlipXY       = 7,
   L_ImgKrnRotateFlipType_Rotate180FlipNone    = L_ImgKrnRotateFlipType_RotateNoneFlipXY,
   L_ImgKrnRotateFlipType_Rotate180FlipX       = L_ImgKrnRotateFlipType_RotateNoneFlipY,
   L_ImgKrnRotateFlipType_Rotate180FlipY       = L_ImgKrnRotateFlipType_RotateNoneFlipX,
   L_ImgKrnRotateFlipType_Rotate180FlipXY      = L_ImgKrnRotateFlipType_RotateNoneFlipNone,
   L_ImgKrnRotateFlipType_Rotate270FlipNone    = L_ImgKrnRotateFlipType_Rotate90FlipXY,
   L_ImgKrnRotateFlipType_Rotate270FlipX       = L_ImgKrnRotateFlipType_Rotate90FlipY,
   L_ImgKrnRotateFlipType_Rotate270FlipY       = L_ImgKrnRotateFlipType_Rotate90FlipX,
   L_ImgKrnRotateFlipType_Rotate270FlipXY      = L_ImgKrnRotateFlipType_Rotate90FlipNone
};
typedef enum L_ImgKrnRotateFlipType L_ImgKrnRotateFlipType;

struct L_ImgKrnPerspectiveCorrectionData
{
    L_UINT   StructSize;
   L_POINT   InputPoints[4];
   L_POINT   MappingPoints[4];
};
typedef struct L_ImgKrnPerspectiveCorrectionData L_ImgKrnPerspectiveCorrectionData;

struct L_ImgKrnImage
{
   L_UINT                StructSize;
   L_UINT                Width;
   L_UINT                Height;
   L_ImgKrnImageFormat   ImageFormat;
   L_UCHAR*              ImageData;
};
typedef struct L_ImgKrnImage L_ImgKrnImage;

struct L_ImgKrnCreateImageOptions
{
   L_UINT   StructSize;
   L_BOOL   CopyData;
};
typedef struct L_ImgKrnCreateImageOptions L_ImgKrnCreateImageOptions;

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnCreateImage(BITMAPHANDLE* bitmap, L_UINT structSize, const L_ImgKrnImage* image, const L_ImgKrnCreateImageOptions* options);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnFromYUV(BITMAPHANDLE* bitmap, L_UINT structSize, const YUVIMAGE* yuvImage, const L_ImgKrnCreateImageOptions* options);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnCopyImage(BITMAPHANDLE* srcBitmap, BITMAPHANDLE* destBitmap, L_UINT structSize);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnSignalToNoiseRatio(BITMAPHANDLE* bitmap, L_DOUBLE* ratio);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnDetectGlare(BITMAPHANDLE* bitmap, L_RECT* glareArea);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnDetectDocument(BITMAPHANDLE* bitmap, L_POINT* documentArea);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnDetectBusinessCard(BITMAPHANDLE* bitmap, L_POINT* businessCardArea);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnRotateFlipImage(BITMAPHANDLE* bitmap, L_ImgKrnRotateFlipType type);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnCropImage(BITMAPHANDLE* bitmap, const L_RECT* rect);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnInvertImage(BITMAPHANDLE* bitmap);

L_LTIMGKRN_API L_INT EXT_FUNCTION L_ImgKrnManualPerspectiveCorrection(BITMAPHANDLE* srcBitmap, const L_ImgKrnPerspectiveCorrectionData* data, BITMAPHANDLE* destBitmap, L_UINT structSize);

#undef L_HEADER_ENTRY
#include "ltpck.h"

#endif // #if defined(LEADTOOLS_V19_OR_LATER)

#endif // #if !defined(LTIMGKRN_H)
