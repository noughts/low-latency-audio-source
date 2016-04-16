mkdir -p Assets/StreamingAssets/Resources/Sounds/
cp -r Assets/example/Resources/Sounds/ Assets/StreamingAssets/Resources/Sounds/

cd Assets/StreamingAssets/Resources/Sounds/
for file in `\find . -maxdepth 1 -type f`; do
    echo $file
done
