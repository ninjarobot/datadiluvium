echo "Generating Hugo Site with landing-page-hugo."
hugo --theme=landing-page-hugo
echo "Copying latest published content."
tar cvfz datadiluvium.tgz public/*
mv datadiluvium.tgz ../datadiluvium.tgz
cd ..
tar -xf datadiluvium.tgz
