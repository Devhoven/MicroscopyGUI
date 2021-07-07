//
//  LTCodecsRtfOptions.h
//  Leadtools.Codecs
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterColor.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTCodecsRtfLoadOptions : NSObject

@property (nonatomic, copy) LTRasterColor *backColor;

- (instancetype)init __unavailable;

@end



NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTCodecsRtfOptions : NSObject

@property (nonatomic, strong, readonly) LTCodecsRtfLoadOptions *load;

- (instancetype)init __unavailable;

@end

NS_ASSUME_NONNULL_END
