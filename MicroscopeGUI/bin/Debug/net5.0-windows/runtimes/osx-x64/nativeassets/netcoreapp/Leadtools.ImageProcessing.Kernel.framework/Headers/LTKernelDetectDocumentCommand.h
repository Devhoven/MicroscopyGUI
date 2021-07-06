//
//  LTKernelDetectDocumentCommand.h
//  Leadtools.ImageProcessing.Kernel
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>
#import <Leadtools/LTPrimitives.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTKernelDetectDocumentCommand : LTRasterCommand

@property (nonatomic, strong, readonly, nullable) NSArray<NSValue *> *documentArea; // LeadPoint[4]

@end

NS_ASSUME_NONNULL_END
