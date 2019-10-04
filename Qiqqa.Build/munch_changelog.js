var fs = require('fs');
var path = require('path');

var rawlog = fs.readFileSync(path.join(__dirname, "../changelog.dump.tmp.txt"), 'utf8');

// format per record in there:
// 
//   <date>:::
//   commit <shorthash>
//   - <message>
// 

//var record_re = /([0-9]{4}-[0-9]{2}-[0-9]{2}):::\s+Commit ([0-9a-f]+)\s+- (.*?)(?=[0-9]{4}-[0-9]{2}-[0-9]{2}:::)/gi;
var record_re = /([0-9]{4}-[0-9]{2}-[0-9]{2}):::\s+Commit ([0-9a-f]+)\s+- /gi;
//console.log(`Re = ${record_re}.`);

var records = [];
var dates = [0];  // make sure indexes start at 1, so date_hashtable[] checks can be fast 'truthy' checks
var date_hashtable = {};
var i = 0;
var marr;
while ((marr = record_re.exec(rawlog)) !== null) {
  //console.log(`Found ${marr[0]} at ${marr.index}. Next starts at ${record_re.lastIndex}.`);

  let rec = records[i] = {
  	index: marr.index,
  	//leader: marr[0],
  	leader_length: marr[0].length,
  	date: marr[1],
  	commit: marr[2],
  	msg_start: 0,
  	msg_end: 0,
  	msg: null,
  };

  rec.msg_start = rec.index + rec.leader_length;
  if (i > 0)
  {
  	let prev_rec = records[i - 1];
  	prev_rec.msg_end = rec.index;
  }

  let d = marr[1];
  if (!date_hashtable[d])
  {
  	// new date (older): start storing record indexes on that date:
  	let di = date_hashtable[d] = dates.length;
  	dates[di] = [i];
  }
  else
  {
  	// existing date: fetch the index for that one and add the record (index) to its list
  	var di = date_hashtable[d];
  	dates[di].push(i);
  }

  i++;
}

// mark the end of the last record msg:
let last_rec = records[records.length - 1];
last_rec.msg_end = rawlog.length;

// now extract the messages:
for (i = 0; i < records.length; i++)
{
	let rec = records[i];
	rec.msg = rawlog.substring(rec.msg_start, rec.msg_end).trim();
}

// dump the records, chunked per date:
for (i = 0; i < dates.length; i++)
{
	let list = dates[i];
	for (let j = 0; j < list.length; j++)
	{
		let idx = list[j];
		let rec = records[idx];
	
		if (j == 0)
		{
			console.log(`


${rec.date}
----------

			`);
		}

		// preprocess the message: it must be indented 2 spaces
		let msg = rec.msg.split('\n').join('\n  ');

		// if the message STARTS with a bullet, e.g. '+ ', '- ' or '* ', then that list must be started on the next line:
		if (/^[+*-] /.test(msg))
		{
			msg = "\n  " + msg;
		}

		console.log(`* (${rec.commit}) ${msg}
			`);
	}
}

