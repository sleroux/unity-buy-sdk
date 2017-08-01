#!/bin/bash

. $(dirname $0)/common.sh
. $(dirname $0)/native_tests.sh

check() {
    if [ $? = 0 ] ; then
        echo "Export $@ finished properly."
    else
        echo "Export $@ failed. Exited with $?"
        echo "------------------\n\n"
        cat $UNITY_LOG_PATH 

        exit 1
    fi
}

# delete all files required for native testing
delete_native_tests

# check if we need to do a major, minor, patch update
VERSION=`cat $SCRIPTS_ROOT/version`

VERSION_ARRAY=( ${VERSION//./ } )
PUBLISH_DESTINATION=$2
UNITY_PACKAGE=shopify-buy.unitypackage

if [ $# -eq 0 ] ; then
    echo "If you'd like to bump versions pass: major, minor, or patch. Using $VERSION for now."
elif [ $1 = "major" ] ; then
    ((VERSION_ARRAY[0]++))
    VERSION_ARRAY[1]=0
    VERSION_ARRAY[2]=0
elif [ $1 = "minor" ] ; then
    ((VERSION_ARRAY[1]++))
    VERSION_ARRAY[2]=0
elif [ $1 = "patch" ] ; then
    ((VERSION_ARRAY[2]++))
elif [ $1 = "github" -o $1 = "asset-store" ] ; then
    PUBLISH_DESTINATION=$1
else
    echo -e "\nInvalid first parameter: \"$1\". You must pass in either a version bump param or a publish destination:\nmajor,\nminor,\npatch,\ngithub,\nasset-store"
    exit 1
fi

if [ "$PUBLISH_DESTINATION" != "github" ] && [ "$PUBLISH_DESTINATION" != "asset-store" ] ; then
    die "\nInvalid publish target.\n\nPublish target should be:\n\"asset-store\"\n\"github\"\n\nExample: scripts/publish.sh github\n"
fi

VERSION="${VERSION_ARRAY[0]}.${VERSION_ARRAY[1]}.${VERSION_ARRAY[2]}"

echo $VERSION > $SCRIPTS_ROOT/version
echo "Version used $1: $VERSION"

# Run generate.sh to create source code including the potentially new version number
$SCRIPTS_ROOT/generate.sh $PUBLISH_DESTINATION
check "generate"

# Run tests just in case
$SCRIPTS_ROOT/test.sh
check "test"

# Now we'll attempt to actually generate the unitypackage
UNITY_LOG_PATH=$PROJECT_ROOT/export.log

which $UNITY_PATH &> /dev/null || die "Unity does not exist at $UNITY_PATH" 

$SCRIPTS_ROOT/build_documentation.sh
check "docs"

# copy EXAMPLES.md to Assets/Shopify/examples.txt
cp $PROJECT_ROOT/EXAMPLES.md $PROJECT_ROOT/Assets/Shopify/examples.txt

if [ "$PUBLISH_DESTINATION" = "asset-store" ] ; then
    UNITY_PACKAGE=shopify-buy-asset-store.unitypackage
fi

echo "Exporting the unitypackage at: $UNITY_PACKAGE"

# create the new unitypackage
$UNITY_PATH \
    -batchmode \
    -nographics \
    -silent-crashes \
    -logFile $UNITY_LOG_PATH \
    -projectPath $PROJECT_ROOT \
    -exportPackage Assets/Shopify Assets/Plugins $UNITY_PACKAGE \
    -quit

PUBLISH_SUCCESS=$?

# restore files used for native extensions
restore_native_tests

if [[ $PUBLISH_SUCCESS = 0 ]] ; then
    # clean up examples.txt
    rm $PROJECT_ROOT/Assets/Shopify/examples.txt

    echo "Export finished"
else
    echo "Export failed. Exited with $?"
    echo "------------------\n\n"
    cat $UNITY_LOG_PATH 

    exit 1
fi
