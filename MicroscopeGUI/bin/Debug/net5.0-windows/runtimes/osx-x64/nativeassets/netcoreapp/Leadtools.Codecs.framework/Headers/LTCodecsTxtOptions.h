//
//  LTCodecsTxtOptions.h
//  Leadtools.Codecs
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterColor.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTCodecsTxtLoadOptions : NSObject

@property (nonatomic, assign) BOOL enabled;
@property (nonatomic, assign) BOOL bold;
@property (nonatomic, assign) BOOL italic;
@property (nonatomic, assign) BOOL underline;
@property (nonatomic, assign) BOOL strikethrough;
@property (nonatomic, assign) BOOL useSystemLocale;

@property (nonatomic, assign) NSInteger fontSize;

@property (nonatomic, copy)   NSString *faceName;

@property (nonatomic, copy)   LTRasterColor *fontColor;
@property (nonatomic, copy)   LTRasterColor *highlight;
@property (nonatomic, copy)   LTRasterColor *backColor;

- (instancetype)init __unavailable;

@end



NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTCodecsTxtOptions : NSObject

@property (nonatomic, strong, readonly) LTCodecsTxtLoadOptions *load;

- (instancetype)init __unavailable;

@end

NS_ASSUME_NONNULL_END
