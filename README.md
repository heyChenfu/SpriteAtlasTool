#  Introduction
Use this tool to quickly generate SpriteAtlas from images in a specified file or folder, without the need to drag and drop images for each atlas configuration

Open SpriteAtlasCollectorWindow in menu SpriteAtlasTool/SpriteAtlasCollectorWindow, add and manage atlas configurations in the window

After all configuration is complete, click "Pack SpriteAtlas" to build SpriteAtlas to Output path

The generate SpriteAtlas default uncheck "Include in Build", So it will need script handle SpriteAtlasManager.atlasRequested and SpriteAtlasManager.atlasRegistered event with your own assetbundle management