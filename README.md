# 2DGame
Isometric demo game made on UWP with Win2D.

## How to run

This project uses a signing certificate for deployment (required for UWP apps). For security and to avoid certificate expiration issues, the certificate file is not included in the repository.

### How to Generate a New Certificate

1. **Clone the Repository and Open the Solution:**
   - Open the solution in Visual Studio.

2. **Open the Package Manifest:**
   - In Solution Explorer, double-click on `Package.appxmanifest`.
   - Switch to the **Packaging** tab.

3. **Create a Test Certificate:**
   - Click on the **Choose Certificate...** button.
   - In the dialog, select **Create a test certificate...**.
   - Enter a password when prompted and click **OK**.

4. **Rebuild the Project:**
   - Save the manifest and rebuild the project. The new certificate will be used for signing.

*Note:* This certificate is for development purposes only.