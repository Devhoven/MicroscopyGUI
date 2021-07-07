//
//  LTApplyLinearVoiLookupTableCommand.h
//  Leadtools.ImageProcessing.Core
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>
#import <Leadtools.ImageProcessing.Core/LTEnums.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTApplyLinearVoiLookupTableCommand : LTRasterCommand

@property (nonatomic, assign) double center;
@property (nonatomic, assign) double width;
@property (nonatomic, assign) LTVoiLookupTableCommandFlags flags;

- (instancetype)initWithCenter:(double)center width:(double)width flags:(LTVoiLookupTableCommandFlags)flags;

@end

NS_ASSUME_NONNULL_END
