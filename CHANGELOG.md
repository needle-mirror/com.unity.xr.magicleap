# Changelog

## [3.0.0-preview.5] - 2019-06-11
- Fix the native controller api loader to properly reference `ml_perception_client` instead of `ml_input`
- Fix an issue that prevented the Display provider from properly initializing in Editor using ML Remote
- Disable some old testing menu items
- Fix a couple cases where the UnityMagicLeap plugin would crash because it couldn't load the ML Remote libraries
- Add Multipass support for ML Remote on OSX
- Fix a bug where ML Remote / Zero Iteration on device would silently fail when using the XR SDK implementation
- Add some native support for managing controller feedback

## [3.0.0-preview.4] - 2019-05-20
- Update yamato configuration
- Improve how various ML input devices are handled via XR Input
- Simplify ML Remote library loading in the native plugin

## [3.0.0-preview.3] - 2019-05-18
- Update third party notices

## [3.0.0-preview.2] - 2019-05-17

## [3.0.0-preview.1] - 2019-05-17
- Add support for Unity 2019.2
- Add support for XR Display Subsystem
- Remove disabled clipping plane enforcement toggles
- Add support for hand tracking
- Add Manifest Editor UI
- Update package to build against 0.20.0 MLSDK
- Add support for starting / stopping ML Remote server headlessly via the Unity TestRunner
- Add standalone Gestures subsystem
- Do not fail when requesting confidence for a zero-vertex mesh
- Don't generate colliders for point cloud style meshes

## [2.0.0-preview.14] - 2019-03-05
- Initial Production release
- Fix a number of issues causing instabilty when using ML Remote