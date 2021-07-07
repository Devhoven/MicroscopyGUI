//
//  LTCodecsXlsOptions.h
//  Leadtools.Codecs
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTCodecsXlsLoadOptions : NSObject

@property (nonatomic, assign) BOOL multiPageSheet;
@property (nonatomic, assign) BOOL disableCellClipping;
@property (nonatomic, assign) BOOL showHiddenSheet;
@property (nonatomic, assign) BOOL multiPageUseSheetWidth;
@property (nonatomic, assign) BOOL pageOrderDownTheOver;
@property (nonatomic, assign) BOOL multiPageEnableMargins;

- (instancetype)init __unavailable;

@end



NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTCodecsXlsOptions : NSObject

@property (nonatomic, strong, readonly) LTCodecsXlsLoadOptions *load;

- (instancetype)init __unavailable;

@end

NS_ASSUME_NONNULL_END
