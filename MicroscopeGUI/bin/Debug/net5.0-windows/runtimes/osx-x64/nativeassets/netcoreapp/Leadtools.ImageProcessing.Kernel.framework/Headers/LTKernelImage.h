//
//  LTKernelImage.h
//  Leadtools.ImageProcessing.Kernel
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterImage.h>

typedef NS_ENUM(NSInteger, LTKernelImageFormat) {
    LTKernelImageFormatRGB888 = 0,
    LTKernelImageFormatBGR888,
    LTKernelImageFormatRGB8888,
    LTKernelImageFormatBGR8888,
    LTKernelImageFormatYV12,
    LTKernelImageFormatNV12,
    LTKernelImageFormatNV21,
    LTKernelImageFormatYUY2
};

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTKernelImage : NSObject

@property (nonatomic, assign)           NSInteger width;
@property (nonatomic, assign)           NSInteger height;

@property (nonatomic, assign)           LTKernelImageFormat format;

@property (nonatomic, strong, nullable) NSData *data;

@end



@interface LTRasterImage (KernelImage)

- (nullable instancetype)initWithKernelImage:(LTKernelImage *)image copyData:(BOOL)flag error:(NSError **)error;

@end

NS_ASSUME_NONNULL_END
