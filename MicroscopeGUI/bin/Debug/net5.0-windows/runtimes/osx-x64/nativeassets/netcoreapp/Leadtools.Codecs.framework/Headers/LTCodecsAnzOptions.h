//
//  LTCodecsAnzOptions.h
//  Leadtools.Codecs
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

typedef NS_ENUM(NSInteger, LTCodecsAnzView) {
    LTCodecsAnzViewTransverse,
    LTCodecsAnzViewSagittal,
    LTCodecsAnzViewCoronal
};

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTCodecsAnzLoadOptions : NSObject

@property (nonatomic, assign) LTCodecsAnzView view;

- (instancetype)init __unavailable;

@end



NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTCodecsAnzOptions : NSObject

@property (nonatomic, strong, readonly) LTCodecsAnzLoadOptions *load;

- (instancetype)init __unavailable;

@end

NS_ASSUME_NONNULL_END
