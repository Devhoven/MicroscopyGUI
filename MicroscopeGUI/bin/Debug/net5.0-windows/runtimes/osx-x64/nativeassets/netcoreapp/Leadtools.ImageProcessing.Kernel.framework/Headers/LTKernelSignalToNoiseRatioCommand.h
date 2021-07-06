//
//  LTKernelSignalToNoiseRatioCommand.h
//  Leadtools.ImageProcessing.Kernel
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTKernelSignalToNoiseRatioCommand : LTRasterCommand

@property (nonatomic, assign, readonly) double ratio;

@end

NS_ASSUME_NONNULL_END
