# timestamp=`date mm-dd-yy-HHMM`
timestamp=$( date +%m-%d-%y-%H%M )
echo $timestamp
filename="datadiluvium-$timestamp.tgz"

echo "Generating Hugo Site with landing-page-hugo and cleaning up .DS_Store cruft."
hugo --theme=landing-page-hugo
find . -name '.DS_Store' -type f -delete
echo "Copying latest published content."
tar cvfz $filename public/
echo "Moving content to docs directory."
cp $filename ../datadiluvium.tgz

cd ..
tar -xf datadiluvium.tgz
rm -rf datadiluvium.tgz
cd ..
mv -v docs/public/* docs
cd docs
rm -rf public
cd ../
git add -A
git commit -m 'The latest hugo site publishing, now @ $( date +%m-%d-%y-%H%M ).'
git push origin master
