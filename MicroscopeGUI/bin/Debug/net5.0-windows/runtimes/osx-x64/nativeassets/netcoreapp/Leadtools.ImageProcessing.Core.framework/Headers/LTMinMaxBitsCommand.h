//
//  LTMinMaxBitsCommand.h
//  Leadtools.ImageProcessing.Core
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTMinMaxBitsCommand : LTRasterCommand

@property (nonatomic, assign, readonly) NSInteger minimumBit;
@property (nonatomic, assign, readonly) NSInteger maximumBit;

@end
