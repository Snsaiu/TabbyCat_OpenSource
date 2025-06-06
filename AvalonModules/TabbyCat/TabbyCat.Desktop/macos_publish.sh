#This is a sh script for Mac deployment, create a script with the code in your project root folder and run it from the terminal. You might have to add changes for your project and type.

#!/bin/bash

# Set the project name to YourProjectName for the executable
PROJECT_NAME="TabbyCat.Desktop"
OUTPUT_DIR="./bin/Release/net9.0/osx-arm64/publish"

# Build for arm64
echo "Building for osx-arm64...  $OUTPUT_DIR"
dotnet publish -r osx-arm64 -c Release

# Create the .app bundle
echo "Creating the .app bundle..."

# Define paths
APP_NAME="TabbyCat.app"  # Space in app name is fine
CONTENT_PATH="$APP_NAME/Contents"
MACOS_PATH="$CONTENT_PATH/MacOS"
RESOURCES_PATH="$CONTENT_PATH/Resources"

# Create directories
mkdir -p "$MACOS_PATH"
mkdir -p "$RESOURCES_PATH"

# Copy files to the bundle
echo "Copying files to app bundle..."
cp "$OUTPUT_DIR/$PROJECT_NAME" "$MACOS_PATH/"
cp -R "$OUTPUT_DIR/" "$MACOS_PATH/"
cp -R "$OUTPUT_DIR/"*.dylib "$MACOS_PATH/" 2>/dev/null || true

# Copy Resources and Assets
if [ -d "$OUTPUT_DIR/Resources" ]; then
    cp -R "$OUTPUT_DIR/Resources" "$RESOURCES_PATH/"
fi

if [ -d "$OUTPUT_DIR/Assets" ]; then
    cp -R "$OUTPUT_DIR/Assets" "$RESOURCES_PATH/"
fi

# Copy icon if it exists
if [ -f "app.icns" ]; then
    echo "Copying app.icns to Resources..."
    cp "app.icns" "$RESOURCES_PATH/"
fi

# After creating directories, add:
echo "Copying credentials and resources..."

# Create Resources directory in both locations
mkdir -p "$MACOS_PATH/Resources"
mkdir -p "$RESOURCES_PATH"

# Copy icon if it exists
if [ -f "app.icns" ]; then
    echo "Copying app.icns to Resources..."
    cp "app.icns" "$RESOURCES_PATH/"
fi

# Create Info.plist
cat <<EOF > "$CONTENT_PATH/Info.plist"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
    <dict>
        <key>CFBundleIconFile</key>
        <string>app.icns</string>
        <key>CFBundleIdentifier</key>
        <string>com.youyan.tabbycat</string>
        <key>CFBundleName</key>
        <string>TabbyCat</string>
        <key>CFBundleVersion</key>
        <string>1.0.0</string>
        <key>LSMinimumSystemVersion</key>
        <string>10.12</string>
        <key>CFBundleExecutable</key>
        <string>TabbyCat.Desktop</string>
        <key>CFBundleInfoDictionaryVersion</key>
        <string>6.0</string>
        <key>CFBundlePackageType</key>
        <string>APPL</string>
        <key>CFBundleShortVersionString</key>
        <string>1.0</string>
        <key>NSHighResolutionCapable</key>
        <true/>
    </dict>
</plist>
EOF

# Update your entitlements.plist creation:
cat <<EOF > "$CONTENT_PATH/entitlements.plist"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>com.apple.security.cs.allow-jit</key>
    <true/>
    <key>com.apple.security.cs.allow-unsigned-executable-memory</key>
    <true/>
    <key>com.apple.security.cs.disable-library-validation</key>
    <true/>
    <key>com.apple.security.cs.allow-dyld-environment-variables</key>
    <true/>
    <key>com.apple.security.network.client</key>
    <true/>
    <key>com.apple.security.network.server</key>
    <true/>
    <key>com.apple.security.files.user-selected.read-write</key>
    <true/>
</dict>
</plist>
EOF

# Make executable
echo "Making application executable..."
chmod +x "$MACOS_PATH/TabbyCat.Desktop"

# Sign the application
echo "Signing application..."
codesign --deep --force --options runtime \
    --entitlements "$CONTENT_PATH/entitlements.plist" \
    --sign "Developer ID Application: liujun fang (7443K5WM8B)" \
    "$APP_NAME"

if [ $? -ne 0 ]; then
    echo "Signing failed"
    exit 1
fi


# Create DMG for distribution
echo "Creating DMG for distribution..."
hdiutil create -volname "TabbyCat.Desktop" -srcfolder "$APP_NAME" -ov -format UDZO "TabbyCat.dmg"

if [ $? -ne 0 ]; then
    echo "DMG creation failed"
    exit 1
fi

# Submit DMG for notarization
echo "Submitting DMG for notarization..."
xcrun notarytool submit "TabbyCat.dmg" \
    --wait \
    --apple-id "snsaiu@outlook.com" \
    --password "ozvk-mzka-vqlg-ayku" \
    --team-id "7443K5WM8B"

if [ $? -ne 0 ]; then
    echo "Notarization submission failed"
    exit 1
fi

# Staple the notarization to the DMG
echo "Stapling notarization to DMG..."
xcrun stapler staple "TabbyCat.dmg"

if [ $? -ne 0 ]; then
    echo "Stapling failed"
    exit 1
fi

echo "All steps completed successfully."

