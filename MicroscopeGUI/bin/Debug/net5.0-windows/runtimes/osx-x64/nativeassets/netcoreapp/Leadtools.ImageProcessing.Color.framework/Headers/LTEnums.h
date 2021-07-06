//
//  LTEnums.h
//  Leadtools.ImageProcessing.Color
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

typedef NS_ENUM(NSInteger, LTGrayScaleToDuotoneCommandMixingType) {
    LTGrayScaleToDuotoneCommandMixingTypeMixWithOldValue   = 0x00000000,
    LTGrayScaleToDuotoneCommandMixingTypeReplaceOldWithNew = 0x00000001
};

typedef NS_ENUM(NSInteger, LTHistogramEqualizeType) {
    LTHistogramEqualizeTypeNone = 0x0000,
    LTHistogramEqualizeTypeRgb  = 0x0001,
    LTHistogramEqualizeTypeYuv  = 0x0002,
    LTHistogramEqualizeTypeGray = 0x0004
};
