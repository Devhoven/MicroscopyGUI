//
//  Leadtools.ImageProcessing.Kernel.h
//  Leadtools.ImageProcessing.Kernel
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#if !defined(LEADTOOLS_IMAGEPROCESSING_KERNEL_FRAMEWORK)
#define LEADTOOLS_IMAGEPROCESSING_KERNEL_FRAMEWORK

#import <Leadtools.ImageProcessing.Kernel/LTKernelCopyImageCommand.h>
#import <Leadtools.ImageProcessing.Kernel/LTKernelCropCommand.h>
#import <Leadtools.ImageProcessing.Kernel/LTKernelDetectDocumentCommand.h>
#import <Leadtools.ImageProcessing.Kernel/LTKernelDetectGlareCommand.h>
#import <Leadtools.ImageProcessing.Kernel/LTKernelInvertCommand.h>
#import <Leadtools.ImageProcessing.Kernel/LTKernelManualPerspectiveCorrectionCommand.h>
#import <Leadtools.ImageProcessing.Kernel/LTKernelPerspectiveDeskewCommand.h>
#import <Leadtools.ImageProcessing.Kernel/LTKernelRotateFlipCommand.h>
#import <Leadtools.ImageProcessing.Kernel/LTKernelSignalToNoiseRatioCommand.h>

#import <Leadtools.ImageProcessing.Kernel/LTKernelImage.h>
#import <Leadtools.ImageProcessing.Kernel/LTRasterCommandsList.h>

// Versioning
#import <Leadtools/LTLeadtools.h>

LEADTOOLS_EXPORT const unsigned char LeadtoolsImageProcessingKernelVersionString[];
LEADTOOLS_EXPORT const double LeadtoolsImageProcessingKernelVersionNumber;

#endif // #if !defined(LEADTOOLS_IMAGEPROCESSING_KERNEL_FRAMEWORK)
