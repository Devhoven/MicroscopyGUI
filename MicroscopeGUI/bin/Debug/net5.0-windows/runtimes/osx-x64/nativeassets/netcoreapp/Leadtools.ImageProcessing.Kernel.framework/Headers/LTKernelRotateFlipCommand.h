//
//  LTKernelRotateFlipCommand.h
//  Leadtools.ImageProcessing.Kernel
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>

typedef NS_ENUM(NSInteger, LTKernelRotateFlipType) {
    LTKernelRotateFlipTypeRotateNoneFlipNone = 0,
    LTKernelRotateFlipTypeRotateNoneFlipX    = 1,
    LTKernelRotateFlipTypeRotateNoneFlipY    = 2,
    LTKernelRotateFlipTypeRotateNoneFlipXY   = 3,
    LTKernelRotateFlipTypeRotate90FlipNone   = 4,
    LTKernelRotateFlipTypeRotate90FlipX      = 5,
    LTKernelRotateFlipTypeRotate90FlipY      = 6,
    LTKernelRotateFlipTypeRotate90FlipXY     = 7,
    LTKernelRotateFlipTypeRotate180FlipNone  = LTKernelRotateFlipTypeRotateNoneFlipXY,
    LTKernelRotateFlipTypeRotate180FlipX     = LTKernelRotateFlipTypeRotateNoneFlipY,
    LTKernelRotateFlipTypeRotate180FlipY     = LTKernelRotateFlipTypeRotateNoneFlipX,
    LTKernelRotateFlipTypeRotate180FlipXY    = LTKernelRotateFlipTypeRotateNoneFlipNone,
    LTKernelRotateFlipTypeRotate270FlipNone  = LTKernelRotateFlipTypeRotate90FlipXY,
    LTKernelRotateFlipTypeRotate270FlipX     = LTKernelRotateFlipTypeRotate90FlipY,
    LTKernelRotateFlipTypeRotate270FlipY     = LTKernelRotateFlipTypeRotate90FlipX,
    LTKernelRotateFlipTypeRotate270FlipXY    = LTKernelRotateFlipTypeRotate90FlipNone
};

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTKernelRotateFlipCommand : LTRasterCommand

@property (nonatomic, assign) LTKernelRotateFlipType type;

- (instancetype)initWithType:(LTKernelRotateFlipType)type NS_DESIGNATED_INITIALIZER;

@end

NS_ASSUME_NONNULL_END
