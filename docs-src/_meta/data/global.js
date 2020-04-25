
module.exports = function() {
  console.log("############## global data:", arguments);

  return {
	// layout: "default",      <-- bloody useless as this ends up at key `global.layout`
  };
};
