package hu.bme.mit.trainbenchmark.ttc.benchmark.emf;

import hu.bme.mit.trainbenchmark.ttc.benchmark.benchmarkcases.AbstractMatch;
import hu.bme.mit.trainbenchmark.ttc.benchmark.benchmarkcases.AbstractSwitchSensorMatch;
import hu.bme.mit.trainbenchmark.ttc.railway.Switch;

public class EMFSwitchSensorMatch extends AbstractSwitchSensorMatch<Switch> {

	public EMFSwitchSensorMatch(final Switch sw) {
		super(sw);
	}

	@Override
	public int compareTo(final AbstractMatch m1) {
		if (m1 instanceof EMFRouteSensorMatch) {
			return Integer.compare(sw.getId(), ((EMFSwitchSensorMatch) m1).getSw().getId());
		} else {
			return -1;
		}
	}

}
