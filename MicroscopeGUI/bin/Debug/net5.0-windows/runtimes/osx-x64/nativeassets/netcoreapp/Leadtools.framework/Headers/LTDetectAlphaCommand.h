//
//  LTDetectAlphaCommand.h
//  Leadtools
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>

/**
 @brief Determines if the image has meaningful alpha channel values.
 */
NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTDetectAlphaCommand : LTRasterCommand

/** @brief Specifies whether or not the image contains meaningful alpha channel values. */
@property (nonatomic, assign) BOOL hasMeaningfulAlpha;

@end
