//
//  LTShiftDataCommand.h
//  Leadtools.ImageProcessing.Core
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>
#import <Leadtools/LTRasterImage.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTShiftDataCommand : LTRasterCommand

@property (nonatomic, strong, readonly, nullable) LTRasterImage *destinationImage;
@property (nonatomic, assign)                     NSUInteger sourceLowBit;
@property (nonatomic, assign)                     NSUInteger sourceHighBit;
@property (nonatomic, assign)                     NSUInteger destinationLowBit;
@property (nonatomic, assign)                     NSUInteger destinationBitsPerPixel;

- (instancetype)initWithSourceLowBit:(NSUInteger)sourceLowBit sourceHighBit:(NSUInteger)sourceHighBit destinationLowBit:(NSUInteger)destinationLowBit destinationBitsPerPixel:(NSUInteger)destinationBitsPerPixel NS_DESIGNATED_INITIALIZER;

@end

NS_ASSUME_NONNULL_END
