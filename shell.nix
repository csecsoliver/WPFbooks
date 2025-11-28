{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  name = "wpf-wine-env";

  # Dependencies
  buildInputs = with pkgs; [
    dotnet-sdk_9
    wineWowPackages.staging
    winetricks
  ];

  # Setup and env
  shellHook = ''
    # 1. Localize Wine to this directory so it doesn't mess with ~/.wine
    export WINEPREFIX="$(pwd)/.wine-prefix"
    export WINEARCH=win64

    # 3. Initialization Logic
    if [ ! -d "$WINEPREFIX" ]; then
      echo "Creating local Wine Prefix (64-bit 32-bit combo)..."
      wineboot --init

      echo "Installing Core Fonts & D3DCompiler (This takes time)..."
      # -q = quiet/unattended mode
      winetricks -q corefonts cjkfonts d3dcompiler_47
      wget https://builds.dotnet.microsoft.com/dotnet/Sdk/9.0.308/dotnet-sdk-9.0.308-win-x64.exe
      wine dotnet-sdk-9.0.308-win-x64.exe
      rm dotnet-sdk-9.0.308-win-x64.exe
      echo "Disabling WPF Hardware Acceleration (Fixes OpenGL crashes)..."
      echo 'REGEDIT4
      [HKEY_CURRENT_USER\Software\Microsoft\Avalon.Graphics]
      "DisableHWAcceleration"=dword:00000001' > disable_hw.reg
      wine regedit disable_hw.reg
      rm disable_hw.reg

      echo "Environment initialized."
    fi

    # 4. Helper Aliases
	alias 'dotnet-wine'='DOTNET_ROOT="$(pwd)/.wine-prefix/drive_c/Program Files/dotnet" && wine'
	dotnet-wpf-build() {
	    local currentPath=$(pwd)
	    local winePath=$(echo $currentPath | sed 's#/#\\#g' | sed 's#^#Z:\\#')
	    # Find the first .csproj file in the current directory
	    local csprojFile=$(find . -maxdepth 1 -name "*.csproj" -print -quit)
	    if [[ -z "$csprojFile" ]]; then
	        echo "No .csproj file found in the current directory."
	        return 1
	    fi
	    # Convert Linux path to Windows path format for Wine
	    local csprojFileName=$(basename "$csprojFile")
	    local wineCsprojPath=$(echo "$currentPath/$csprojFileName" | sed 's#/#\\#g' | sed 's#^#Z:\\#')
	    echo "Building $csprojFileName..."
	    wine cmd /c dotnet publish "$wineCsprojPath" -c Release -r win-x64 --self-contained false
	  echo "Finished building WPF project:)"
	}
    echo "Environment loaded. Type 'run-app' to build and launch."
  '';
}
