//
//  LTKernelCropCommand.h
//  Leadtools.ImageProcessing.Kernel
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>
#import <Leadtools/LTPrimitives.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTKernelCropCommand : LTRasterCommand

@property (nonatomic, assign) LeadRect cropRect;

- (instancetype)initWithRect:(LeadRect)cropRect NS_DESIGNATED_INITIALIZER;

@end

NS_ASSUME_NONNULL_END
