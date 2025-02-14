# 2DGame

Isometric demo game made on UWP (Universal Windows Platform) with Win2D.

## How to run

This project being developed on UWP requires the device to be in Developer Mode. In case the device is not in Developer Mode, Visual Studio will promt you with instructions on how to enable Developer Mode.

This project uses a signing certificate for deployment (required for UWP apps). For security and to avoid certificate expiration issues, the certificate file is not included in the repository.

### How to Generate a New Certificate

1. **Clone the Repository and Open the Solution:**
   - Open the solution in Visual Studio.

2. **Open the Package Manifest:**
   - In Solution Explorer, double-click on `Package.appxmanifest`.
   - Switch to the **Packaging** tab.

3. **Create a Test Certificate:**
   - Click on the **Choose Certificate...** button.
   - In the dialog, select **Create...**.
   - Enter a password when prompted and click **OK**.

4. **Rebuild the Project:**
   - Save the manifest and rebuild the project. The new certificate will be used for signing.
   
5. **Run the Project:**
   - Press Ctrl+F5 to start without debugging

*Note:* This certificate is for development purposes only.

## Controls

- W, A, S, D movement in cardinal directions
- Left click shoot an arrow in the direction of the cursor
- Tab toggle minimap that updates realtime as the player explores the map

## Video sample of the running game

