//
//  LTTissueEqualizeCommand.h
//  Leadtools.ImageProcessing.Core
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>

typedef NS_OPTIONS(NSUInteger, LTTissueEqualizeCommandFlags) {
    LTTissueEqualizeCommandFlagsUseSimplifyOption  = 0x00000001,
    LTTissueEqualizeCommandFlagsUseIntensifyOption = 0x00000002,
};

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTTissueEqualizeCommand : LTRasterCommand

@property (nonatomic, assign) LTTissueEqualizeCommandFlags flags;

- (instancetype)initWithFlags:(LTTissueEqualizeCommandFlags)flags;

@end

NS_ASSUME_NONNULL_END
