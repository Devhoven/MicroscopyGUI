//
//  LTMRZCodeDetectionCommand.h
//  Leadtools.ImageProcessing.Core
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>
#import <Leadtools/LTPrimitives.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTMRZCodeDetectionCommand : LTRasterCommand

@property (nonatomic, assign)           LeadRect searchingZone;
@property (nonatomic, assign, readonly) LeadRect mrzZone;

@end

NS_ASSUME_NONNULL_END
