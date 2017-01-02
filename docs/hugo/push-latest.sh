# timestamp=`date mm-dd-yy-HHMM`
timestamp=$( date +%m-%d-%y-%H%M )
echo $timestamp
filename="datadiluvium-$timestamp.tgz"

echo "Generating Hugo Site with landing-page-hugo and cleaning up .DS_Store cruft.\n * * * * *"
hugo --theme=landing-page-hugo
find . -name '.DS_Store' -type f -delete
echo "Copying latest published content.\n * * * * *"
tar cvfz $filename public/
echo "Moving content to docs directory.\n * * * * *"
cp $filename ../datadiluvium.tgz

cd ..
tar -xf datadiluvium.tgz
rm -rf datadiluvium.tgz
cd ..
echo "Removing old site.\n * * * * *"
rm -rf about css font-awesome-4.1.0 fonts img index.html index.xml js services sitemap.xml
echo "Placing new site.\n * * * * *"
mv -v docs/public/* docs
cd docs
rm -rf public
cd ../
echo "Committing and pushing new site.\n * * * * *"
git add -A
git commit -m 'The latest hugo site publishing from file $filename.'
git push origin master
